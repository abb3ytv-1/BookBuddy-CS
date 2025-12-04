📚 BookBuddy — BookTracker API

BookBuddy is a C# Web API that allows users to manage, organize, and review their personal book collections.
Built with ASP.NET Core, Entity Framework Core, and Docker, it supports secure user authentication, achievements, notifications, and external book search.

🎯 Purpose

This project demonstrates:

RESTful API design in C#

Database integration with Entity Framework Core

Layered architecture: Controllers → Facade → Services → Data

Implementation of design patterns (Facade, State) for maintainable and extensible code

Integration with notifications and external APIs (Hardcover API)

The Facade pattern centralizes business logic, simplifies controllers, and improves maintainability.
The State pattern allows books to manage their own reading statuses (Unread, Reading, Read) in a clean, object-oriented way.

🧩 Features

Track book status: Unread, Reading, Read

Add reviews and ratings

Achievements system when books are read

Notifications for reviews and completed books

Secure endpoints using JWT/Identity

External API integration for searching books

🧰 Setup & Run (Backend)
Requirements

.NET 8 SDK or later

Docker Desktop

Visual Studio 2022 or VS Code

Run Using .NET CLI
cd BookTrackerAPI
dotnet restore
dotnet build
dotnet run

🧠 Architecture & Design
Facade Pattern

The LibraryFacade simplifies controllers by centralizing interactions with:

AppDbContext (database)

UserManager<AppUser> (user data)

AchievementService (achievement tracking)

NotificationSender (user notifications)

Controllers now focus only on HTTP requests/responses, while the facade handles all business logic for:

Updating book statuses

Adding reviews

Awarding achievements

Sending notifications

State Pattern

Book reading statuses are managed with the State Pattern, allowing each book to handle its own state transitions:

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