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

## Used technologies

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

### Other
- Terraform for infrastructure management
- Docker for development database


# Screenshots

![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/7c121dd3-a1be-41a0-87aa-a09aaa905302)

![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/5cbae06f-942e-492a-9734-1e96c31896d7)

![kuva](https://github.com/tommi-miettinen/kissarekisteri/assets/63008431/21b92de3-fc7f-4bb0-b69f-ed03f1794cae)
