📚 BookBuddy — BookTracker API

BookBuddy is a C# Web API that allows users to manage, organize, and review their personal book collections.
Built with ASP.NET Core, Entity Framework Core, and Docker, it supports secure user authentication, achievements, notifications, and external book search.

🎯 Purpose

This project demonstrates:

RESTful API design in C#

Database integration with Entity Framework Core

Layered architecture: Controllers → Facade → Services → Data

Implementation of design patterns (Facade, Singleton, State) for maintainable and extensible code

Integration with notifications and external APIs (Hardcover API)

Key Design Patterns Used:

Facade Pattern: Centralizes business logic, simplifies controllers, and improves maintainability.

Singleton Pattern (LibraryFacade): Ensures a single instance of the facade handles all book-related operations while keeping scoped services thread-safe.

State Pattern: Allows books to manage their own reading statuses (Unread, Reading, Read) cleanly and modularly.

🧩 Features

Track book status: Unread, Reading, Read

Add reviews and ratings

Achievement system triggered by reading milestones

Notifications for reviews and completed books

Secure endpoints using JWT/Identity

External API integration for searching books

🧰 Setup & Run (Backend)

Requirements:

.NET 8 SDK or later

Docker Desktop

Visual Studio 2022 or VS Code

Run Using .NET CLI:

cd BookTrackerAPI
dotnet restore
dotnet build
dotnet run


Open Swagger UI to explore API endpoints:

https://localhost:{port}/swagger

🧠 Architecture & Design
Facade Pattern

The LibraryFacade simplifies controllers by centralizing interactions with:

AppDbContext (database)

UserManager<AppUser> (user data)

AchievementService (achievement tracking)

NotificationSender (user notifications)

Responsibilities:

Updating book statuses

Adding reviews

Awarding achievements

Sending notifications

Singleton Pattern

LibraryFacade is registered as a singleton via dependency injection.

Provides centralized access to library operations across controllers.

Scoped services like DbContext remain safe per request.

Ensures consistent business logic and reduces duplication across the app.

State Pattern

Book reading statuses are managed with the State Pattern:

public void MarkAsRead() => State.MarkAsRead(this);
public void MarkAsUnread() => State.MarkAsUnread(this);
public void MarkAsReading() => State.MarkAsReading(this);


Benefits:

Reduces duplicate logic in controllers

Keeps the model smart and modular

Makes it easy to add future states (e.g., Wishlist, Abandoned)

✅ Benefits

Clean separation of concerns

Simplified, maintainable controllers

Encapsulated business logic

Extensible architecture for future features

🔗 Technologies

ASP.NET Core 7

Entity Framework Core

ASP.NET Identity

C#

SQL Server / SQLite

Hardcover API integration
