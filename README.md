# Kissarekisteri

[Live demo](https://kissarekisteri-app.azurewebsites.net)

## Documentation

- [Role-based Access Control](role-based-access-control.md)
- [Swagger](https://kissarekisteri-app.azurewebsites.net/swagger/index.html)

## Features
- Microsoft Azure B2C Authentication
- Json Web Token Authorization
- Role-based Access Control
- Localization with vue-i18n (English & Finnish)

#### Users can
- Login
- Add cats
- Delete & edit their own cats
- Upload photos for their cats
- Edit their own information (avatar, breeder status)
- Create cat show events (requires EventOrganizer or Admin role)
- Assign cats their placing in the shows (requires EventOrganizer or Admin role)
- Upload photos for cat shows (requires EventOrganizer or Admin role)
- Request ownership of cats that they dont own
- Accept ownership requests of their cats to transfer to the requester
- View and filter cats by name, breed and sex
- View and filter users by name and role
- View and filter cat shows by name and location
- Assign roles to other users (requires Admin role)

## Technologies

### Frontend
- TypeScript
- Vue
- Bootstrap
- Vue-query for api calls and caching
- Microsoft authentication library for login functionality

### Backend
- C#
- ASP.NET

### Database
- Microsoft SQL Server

### Cloud services
- Azure Blob Storage for file storage
- Azure App Services for hosting
- Azure SQL Server for data storage
- Azure Adb2C for authorization & authentication

### Other
- Terraform for infrastructure management
- Docker for development database
- Github Actions for deployment pipeline


# Screenshots

![firefox_81Ay1hMwhh](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/279aecac-f070-4180-9f53-dc76e8521168) ![firefox_rehfvQM8VM](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/79b792ad-4e57-498b-9dd9-7e63f299524e)


