terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }
}

provider "azurerm" {
  features {}
}

variable "sql_admin_password" {
  description = "The admin password for SQL Server"
  type        = string
  sensitive   = true
}

locals  {
rg_name = "kissarekisteri"
location = "northeurope"
}

resource "azurerm_resource_group" "rg" {
  name     = locals.rg_name
  location = locals.location
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


resource "azurerm_sql_server" "sqlserver" {
  name                         = "kissarekisterisqlserver"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "kissarekisterisqladmin"
  administrator_login_password =  var.sql_admin_password
}

resource "azurerm_sql_database" "sql" {
  name                = locals.sql_database_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  server_name         = azurerm_sql_server.sqlserver.name
  
}