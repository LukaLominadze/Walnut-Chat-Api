@echo off

REM create solution
dotnet new sln -n Walnut.Api --force

REM create api project and read project files
copy /Y "Walnut.Api\Program.cs" "vendor\ProjectFiles\Program.cs"
copy /Y "Walnut.Api\appsettings.json" "vendor\ProjectFiles\appsettings.json"
copy /Y "Walnut.Api\appsettings.Development.json" "vendor\ProjectFiles\appsettings.Development.json"
copy /Y "Walnut.Api\Properties\launchSettings.json" "vendor\ProjectFiles\launchSettings.json"
dotnet new webapi -n Walnut.Api --force
copy /Y "vendor\ProjectFiles\Program.cs" "Walnut.Api\Program.cs"
copy /Y "vendor\ProjectFiles\appsettings.json" "Walnut.Api\appsettings.json"
copy /Y "vendor\ProjectFiles\appsettings.Development.json" "Walnut.Api\appsettings.Development.json"
copy /Y "vendor\ProjectFiles\launchSettings.json" "Walnut.Api\Properties\launchSettings.json"

REM create models project
dotnet new classlib -n Walnut.Models --force
erase "Walnut.Models\Class1.cs"

REM create data project
dotnet new classlib -n Walnut.Data --force
erase "Walnut.Data\Class1.cs"

REM add projects to solution
dotnet sln Walnut.Api.sln add Walnut.Api/Walnut.Api.csproj
dotnet sln Walnut.Api.sln add Walnut.Data/Walnut.Data.csproj
dotnet sln Walnut.Api.sln add Walnut.Models/Walnut.Models.csproj

REM add packages to projects
dotnet add Walnut.Api/Walnut.Api.csproj package Swashbuckle.AspNetCore.Swagger
dotnet add Walnut.Api/Walnut.Api.csproj package Swashbuckle.AspNetCore.SwaggerGen
dotnet add Walnut.Api/Walnut.Api.csproj package Swashbuckle.AspNetCore.SwaggerUI
dotnet add Walnut.Api/Walnut.Api.csproj package MySql.EntityFrameworkCore
dotnet add Walnut.Api/Walnut.Api.csproj package MySql.Data
dotnet add Walnut.Api/Walnut.Api.csproj package Microsoft.EntityFrameworkCore
dotnet add Walnut.Api/Walnut.Api.csproj package Microsoft.EntityFrameworkCore.Tools
dotnet add Walnut.Api/Walnut.Api.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add Walnut.Api/Walnut.Api.csproj package System.IdentityModel.Tokens.Jwt
dotnet add Walnut.Api/Walnut.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add Walnut.Api/Walnut.Api.csproj package AutoMapper
dotnet add Walnut.Models/Walnut.Models.csproj package AutoMapper
dotnet add Walnut.Data/Walnut.Data.csproj package MySql.EntityFrameworkCore
dotnet add Walnut.Data/Walnut.Data.csproj package MySql.Data
dotnet add Walnut.Data/Walnut.Data.csproj package Microsoft.EntityFrameworkCore
dotnet add Walnut.Data/Walnut.Data.csproj package Microsoft.EntityFrameworkCore.Tools
dotnet add Walnut.Data/Walnut.Data.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add Walnut.Data/Walnut.Data.csproj package Microsoft.AspNetCore.Identity

REM add references to api project
dotnet add Walnut.Api/Walnut.Api.csproj reference Walnut.Data/Walnut.Data.csproj
dotnet add Walnut.Api/Walnut.Api.csproj reference Walnut.Models/Walnut.Models.csproj

REM add references to data project
dotnet add Walnut.Data/Walnut.Data.csproj reference Walnut.Models/Walnut.Models.csproj
pause