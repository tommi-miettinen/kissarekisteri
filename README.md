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

- Install the Azure CLI tool
- Authenticate using the Azure CLI
- Create a Service Principal
- Set your environment variables

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
- Select "Sign up and sign in"
- Select "Recommended"
- Name the user flow "B2C_1_SIGN_IN_SIGN_UP"
- Select local accounts "Email"
- MFA method "Email"
- MFA enforcement "Off
    
### Grant admin permissions for obtaining tokens.
- Navigate to Azure AdB2C / App registrations / {name of auth app}
- Select "Api permissions" from the sidebar
- Grant Admin consent for {name of auth app}

# Kissarekisteri Role-Based Access Control

## Components

### Roles

```C#
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
```

### Permissions

```C#
public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### Role permissions

A lookup table for checking what permissions a role grants

```C#
    public class RolePermission
    {
        public int Id { get; set; }

        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public string PermissionName { get; set; }
        public int PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
```

### User roles

A lookup table for checking what roles the user has assigned

```C#
    public class UserRole
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
```

## Lookup flow
- Checking what roles the user has assigned
- Looking up Role permissions what permissions the role grants
- Does the permission allow for the action

## Lookup implementation

```C#
 public async Task<List<Permission>> GetPermissions(string userId)
 {
     var userRoles = await _dbContext.UserRoles
         .Where(userRoles => userRoles.UserId == userId)
         .ToListAsync();
     var userRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

     var rolePermissions = await _dbContext.RolePermissions
         .Where(rolePermission => userRoleIds.Contains(rolePermission.RoleId))
         .ToListAsync();

     var permissionIds = rolePermissions.Select(rolePermission => rolePermission.PermissionId)
         .ToList();

     var permissions = await _dbContext.Permissions
         .Where(permission => permissionIds.Contains(permission.Id))
         .ToListAsync();

     return permissions;
 }
```
