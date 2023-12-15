## Development

Prerequisites

- Azure account
- Docker
- az cli
- .NET
- Node
- Visual Studio


### Azure
Navigate to terraform/development folder

    terraform plan -target="azurerm_aadb2c_directory.kissarekisteri"
    terraform apply -target="azurerm_aadb2c_directory.kissarekisteri"
    
Update the terraform azuread provider tenant_id so the ad application will be created inside the AdB2C directory.

```hcl
provider "azuread" {
 # tenant_id = "tenant-id-from-aadb2c"
}
```
Deploy the rest of the resources

    terraform plan
    terraform apply


#### Follow the Azure AdB2C setup
- [Azure AdB2C manual setup](#azure-adb2c-manual-setup)


#### Start the database & blob storage

    docker-compose up

### Backend

Start the app



#### Frontend

    cd frontend
    npm install
    npm run dev
    
## Testing

    npx playwright test

## Deployment

#### Setup az cli, authenticate and set the env vars.

1. - Install the Azure CLI tool
2. - Authenticate using the Azure CLI
3. - Create a Service Principal
4. - Set your environment variables

https://developer.hashicorp.com/terraform/tutorials/azure-get-started/azure-build#authenticate-using-the-azure-cli

#### Create the AdB2C directory.

    terraform plan -target="azurerm_aadb2c_directory.kissarekisteriAdB2C"
    terraform apply -target="azurerm_aadb2c_directory.kissarekisteriAdB2C"

#### Update the terraform azuread provider tenant_id so the ad application will be created inside the AdB2C directory.

```hcl
provider "azuread" {
 # tenant_id = "tenant-id-from-aadb2c"
}
```

#### Create the ad application.

    terraform plan -target="azuread_application.kissarekisteriAuth"
    terraform apply -target="azuread_application.kissarekisteriAuth"

#### Remove the tenant_id so rest of the resources get created in the default directory.

```hcl
provider "azuread" {}
```

#### Create rest of the resources.

    terraform plan
    terraform apply

#### Follow the Azure AdB2C setup
- [Azure AdB2C manual setup](#azure-adb2c-manual-setup)

## Azure AdB2C manual setup

### Create a user flow.
- Switch directory to the AdB2C directory you created.
- Navigate to Azure AdB2C / App registrations
- Select "User flows" from the sidebar
- Click "New user flow"
- Select "Sign up and sign in", use the recommended option
- Name the user flow "B2C_1_SIGN_IN_SIGN_UP"
- Select local accounts "Email"
- MFA method "Email"
- MFA enforcement "Off
    
### Grant admin permissions for obtaining tokens.
- Navigate to Azure AdB2C / App registrations / {name of auth app}
- Select "Api permissions" from the sidebar
- Grant Admin consent for {name of auth app}
