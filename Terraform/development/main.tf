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


locals {
  #https://learn.microsoft.com/en-us/graph/permissions-reference
  openid_scope_id         = "37f7f235-527c-4136-accd-4a02d197296e"
  offline_access_scope_id = "7427e0e9-2fba-42fe-b0c0-848c9e6a8182"
  microsoft_graph_app_id  = "00000003-0000-0000-c000-000000000000"
}

resource "azurerm_resource_group" "rg" {
  name     = "kissarekisteridev"
  location = "northeurope"
}

resource "azurerm_aadb2c_directory" "kissarekisteri" {
  country_code            = "FI"
  data_residency_location = "Europe"
  display_name            = "Kissarekisteridev"
  domain_name             = "kissarekisteridev.onmicrosoft.com"
  resource_group_name     = azurerm_resource_group.rg.name
  sku_name                = "PremiumP1"
}


resource "azuread_application" "kissarekisteri" {
  display_name     = "kissarekisteri-auth-dev"
  sign_in_audience = "AzureADandPersonalMicrosoftAccount"

  api {
    requested_access_token_version = 2
  }

  required_resource_access {
    resource_app_id = local.microsoft_graph_app_id

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
    redirect_uris = ["https://jwt.ms/"]

    implicit_grant {
      access_token_issuance_enabled = true
      id_token_issuance_enabled     = true
    }
  }
}


resource "azuread_service_principal" "example" {
  client_id    = azuread_application.kissarekisteri.client_id
  use_existing = true
}

