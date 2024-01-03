terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.84.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "2.46.0"
    }
  }
}

provider "azurerm" {
  features {}
}

provider "azuread" {
 # tenant_id = "d128e5ef-7125-45c2-8e8c-4fd41c0c862e"
}

/*
variable "sql_admin_password" {
  description = "The admin password for SQL Server"
  type        = string
  sensitive   = true
}
*/

locals {
  #https://learn.microsoft.com/en-us/graph/permissions-reference
  openid_scope_id = "37f7f235-527c-4136-accd-4a02d197296e"
  offline_access_scope_id = "7427e0e9-2fba-42fe-b0c0-848c9e6a8182"
}

resource "azurerm_aadb2c_directory" "kissarekisteriAdB2C" {
  country_code            = "FI"
  data_residency_location = "Europe"
  display_name            = "KissarekisteriAdB2C"
  domain_name             = "kissarekisterib2c.onmicrosoft.com"
  resource_group_name     = azurerm_resource_group.rg.name
  sku_name                = "PremiumP1"
}

data "azuread_application_published_app_ids" "well_known" {}

resource "azuread_application" "kissarekisteriAuth" {
  display_name = "kissarekisteriAuthAdB2C"
  sign_in_audience = "AzureADandPersonalMicrosoftAccount"


  api {
    requested_access_token_version = 2
  }
  
  required_resource_access {
    resource_app_id = data.azuread_application_published_app_ids.well_known.result.MicrosoftGraph

    resource_access {
      id   = local.offline_access_scope_id
      type = "Scope"
    }

    resource_access {
      id   = local.openid_scope_id
      type = "Scope"
    }
  }

  web {
    redirect_uris = ["https://localhost:44316", "https://localhost:5173", azurerm_windows_web_app.appservice.default_site_hostname]

    implicit_grant {
      access_token_issuance_enabled = true
      id_token_issuance_enabled     = true
    }
  }
}



resource "azurerm_resource_group" "rg" {
  name     = "kissarekisteri"
  location = "northeurope"
}

resource "azurerm_storage_account" "sa" {
  name                     = "kissarekisteritf"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"

  blob_properties {
    cors_rule {
      allowed_origins    = ["*"]
      allowed_methods    = ["GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS"]
      allowed_headers    = ["*"]
      exposed_headers    = ["*"]
      max_age_in_seconds = 200
    }
  }
}

resource "azurerm_mssql_server" "sqlserver" {
  name                         = "kissarekisterisqlserver"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "kissarekisterisqladmin"
  administrator_login_password = "4-v3ry-53cr37-p455w0rd"
}


resource "azurerm_mssql_database" "sql" {
  name      = "kissarekisteridb"
  server_id = azurerm_mssql_server.sqlserver.id
  sku_name  = "Basic"
}


resource "azurerm_service_plan" "kissarekisterisp" {
  name                = "kissarekisterisp"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Windows"
  sku_name            = "F1"
}

resource "azurerm_windows_web_app" "appservice" {
  name                = "kissarekisteri-app"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.kissarekisterisp.id
  https_only          = true


  site_config {
    always_on = false
    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v8.0"
    }
  }

  identity {
    type = "SystemAssigned"
  }
}