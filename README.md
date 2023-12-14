## Testing

    npx playwright test

## Development

#### Start the database & blob storage

    docker-compose up

#### Backend

Start the app

#### Frontend

    cd frontend
    npm install
    npm run dev

## Deployment

##### 1. Setup az cli, authenticate and set the env vars.

1. - Install the Azure CLI tool
2. - Authenticate using the Azure CLI
3. - Create a Service Principal
4. - Set your environment variables

https://developer.hashicorp.com/terraform/tutorials/azure-get-started/azure-build#authenticate-using-the-azure-cli

##### Create the AdB2C directory.

    terraform plan -target="azurerm_aadb2c_directory.kissarekisteriAdB2C"
    terraform apply -target="azurerm_aadb2c_directory.kissarekisteriAdB2C"

##### Update the terraform azuread provider tenant_id so the ad application will be created inside the AdB2C directory.

```json
provider "azuread" {
 # tenant_id = "tenant-id-from-aadb2c"
}
```

##### Create the ad application.

`terraform plan -target="azuread_application.kissarekisteriAuth"`
`terraform apply -target="azuread_application.kissarekisteriAuth"`

##### Remove the tenant_id so rest of the resources get created in the default directory.

```json
provider "azuread" {}
```

##### Create rest of the resources.

`terraform plan`
`terraform apply`
