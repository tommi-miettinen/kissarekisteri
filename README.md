## Development

Prerequisites

- [Azure account](https://azure.microsoft.com/en-us/free)
- [Docker](https://docs.docker.com/desktop/install/windows-install/)
- [az cli](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli)
- [Node](https://nodejs.org/en)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [.NET](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net80)


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

Open the project in Visual studio

#### Setup the secrets

- Right click on the project
- Select "Manage user secrets"

  ![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/a27dffed-8c6c-49bd-beaf-4a92e2549006)

- Set Storage connection string
- Set AzureSQL connection string
- Set AzureAdB2C:ClientSecret (Needed to query Users in AdB2C with admin access)

The secret can be found here 
![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/2d817b52-7036-46f7-a48b-3d3ad049088f)



Start the app by clicking

![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/1dc3a318-841d-494b-80c2-4f9b8c1e23fb)

or

    dotnet run watch

You might need [dotnet cli tool](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install)





### Frontend

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

## Input Validation
The ModelValidationFilter ensures that all incoming data to controller actions is validated against the model's Data Annotations. If validation fails, it automatically returns a BadRequest response with the validation errors.

Filters/ModelValidationFilter.cs

```C#
public class ModelValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}
```

Applied to all controllers in Program.cs
```C#
builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add(new ModelValidationFilter());
    });
```

### Example Scenario 

Model with Data Annotations

```C#
public class SampleModel
{
    [Required]
    public string Name { get; set; }

    [Range(1, 100)]
    public int Age { get; set; }
}
```

Controller Action

```C#
[HttpPost]
public IActionResult CreateSample(SampleModel model)
{
}
```

Outcome: If CreateSample receives data that doesn't comply with SampleModel validations (e.g., missing Name), ModelValidationFilter intercepts and returns a BadRequest with details of the validation errors.

# Screenshots

![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/7c121dd3-a1be-41a0-87aa-a09aaa905302)

![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/5cbae06f-942e-492a-9734-1e96c31896d7)

![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/21b92de3-fc7f-4bb0-b69f-ed03f1794cae)






