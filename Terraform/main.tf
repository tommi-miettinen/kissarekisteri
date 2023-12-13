terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.84.0"
    }
  }
}

provider "azurerm" {
  features {}
}

locals {
  rg_name  = "kissarekisteri"
  location = "northeurope"
}



/*
variable sql_admin_password {
  description = "The admin password for SQL Server"
  type        = string
  sensitive   = true
}
*/





resource "azurerm_resource_group" "rg" {
  name     = local.rg_name
  location = local.location
}


resource "azurerm_storage_account" "sa" {
  name                     = "kissarekisteritf"
  resource_group_name      = local.rg_name
  location                 = local.location
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

  depends_on = [azurerm_resource_group.rg]
}


/*
resource azurerm_sql_server sqlserver {
  name                         = "kissarekisterisqlserver"
  resource_group_name          = locals.rg_name
  location                     = locals.rg.location
  version                      = "12.0"
  administrator_login          = "kissarekisterisqladmin"
  administrator_login_password =  var.sql_admin_password
}

resource azurerm_sql_database sql {
  name                = locals.sql_database_name
  resource_group_name = locals.rg_name
  location            = locals.rg.location
  server_name         = azurerm_sql_server.sqlserver.name

}
*/


resource "azurerm_service_plan" "example" {
  name                = "kissarekisterisp"
  location            = local.location
  resource_group_name = local.rg_name
  os_type             = "Windows"
  sku_name            = "F1"

  depends_on = [azurerm_resource_group.rg]
}

resource "azurerm_windows_web_app" "appservice" {
  name                = "kissarekisteri-app"
  location            = local.location
  resource_group_name = local.rg_name
  service_plan_id     = azurerm_service_plan.example.id



  site_config {
    always_on = false
    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v8.0"
    }

  }
  https_only = true

  depends_on = [azurerm_resource_group.rg, azurerm_service_plan.example]
}


output "app_service_publish_profile" {
  description = "Azure App Services publish profile, add this to github secrets for deployment workflow"
  value       = azurerm_windows_web_app.appservice.site_credential
  sensitive   = true
}