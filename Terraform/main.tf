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

provider "azuread" {}

/*
variable "sql_admin_password" {
  description = "The admin password for SQL Server"
  type        = string
  sensitive   = true
}
*/

resource "azurerm_aadb2c_directory" "kissarekisteriAdB2C" {
  data_residency_location = "Europe"
  domain_name             = "kissarekisteri.onmicrosoft.com"
  resource_group_name     = azurerm_resource_group.rg.name
  sku_name                = "PremiumP1"
}

resource "azuread_application" "kissarekisteriAuth" {
  display_name = "test"

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

resource "azurerm_role_assignment" "sqlrole" {
  scope                = azurerm_mssql_server.sqlserver.id
  role_definition_name = "Contributor"
  principal_id         = azurerm_windows_web_app.appservice.identity[0].principal_id
}

/*
resource "null_resource" "sql_script" {
  provisioner "local-exec" {
    command = <<EOT
      az sql db exec --resource-group ${local.rg_name} \
                     --server ${azurerm_sql_server.sqlserver.name} \
                     --name ${azurerm_sql_database.sql.name} \
                     --sql "CREATE USER ${azurerm_windows_web_app.appservice.name} FROM EXTERNAL PROVIDER;"
    EOT
  }

  depends_on = [azurerm_sql_database.sql]
}
*/