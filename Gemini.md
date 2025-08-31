# Gemini Project Context: Socialify

This document provides context for the Socialify project to the Gemini AI assistant.

## 1. Project Overview

Socialify is a social media application built with ASP.NET Core. It appears to follow a Clean Architecture or a similar layered approach, separating concerns into Domain, Application, Infrastructure, and Presentation layers.

The core functionality revolves around users, authentication (login/register), and social media posts.

## 2. Technology Stack

- **Backend:** C#, .NET 8
- **Framework:** ASP.NET Core MVC
- **Data Access:** Entity Framework Core (EF Core)
- **Database:** Microsoft SQL Server (judging by the `UseSqlServer` configuration and packages)
- **Authentication:** ASP.NET Core Identity
- **Object-to-Object Mapping:** AutoMapper
- **Frontend:** Razor Views, Bootstrap, JavaScript (jQuery)

## 3. Architecture

The solution is divided into four main projects:

- `Socialify.Domain`: Contains the core business entities (`ApplicationUser`) and domain interfaces (`IUserRepository`, `IAuthService`). This is the center of the architecture.
- `Socialify.Application`: Holds application logic, DTOs (`LoginDto`, `RegisterDto`, `UserDto`), and AutoMapper profiles. It depends on the Domain layer.
- `Socialify.Infrastructure`: Implements data access and external services. It contains the `SocialifyDbContext`, repository implementations, and the `AuthService`. It depends on the Application layer.
- `Socialify.Presentation`: The user-facing web application (ASP.NET Core MVC project). It contains controllers, views, and static assets (`wwwroot`). It depends on the Infrastructure and Application layers.

## 4. Coding Conventions

- **Asynchronous Programming:** Use `async`/`await` for all I/O-bound operations, especially database and network calls.
- **Dependency Injection:** Services are registered in `Program.cs` and injected into constructors. Follow this pattern for new services.
- **DTOs:** Use Data Transfer Objects (DTOs) for transferring data between the presentation layer and the application layer. Do not expose Domain entities directly to the UI.
- **AutoMapper:** Use AutoMapper for mapping between Domain entities and DTOs.
- **Result Pattern:** The project uses a generic `Result<T>` class to encapsulate the outcome of operations, indicating success or failure with associated data or error messages.
- **Configuration:** Database connection strings and other settings are stored in `appsettings.json`.

## 5. Key Files

- **Startup/Configuration:** `Socialify.Presentation/Program.cs`
- **Database Context:** `Socialify.Infrastructure/Data/Context/SocialifyDbContext.cs`
- **User Entity:** `Socialify.Domain/Entities/ApplicationUser.cs`
- **Authentication Service:** `Socialify.Infrastructure/Identity/AuthService.cs`
