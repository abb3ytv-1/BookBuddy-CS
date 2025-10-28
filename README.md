# 📚 BookBuddy — BookTracker API  

BookBuddy is a C# Web API that allows users to manage, organize, and review their book collections.  
It’s built with **ASP.NET Core**, **Entity Framework**, and **Docker**, with a connected frontend for the user interface.

---

## 🎯 Purpose
This project demonstrates:
- RESTful API design in C#
- Database integration using Entity Framework Core
- Layered architecture (Controllers → Services → Data)
- Containerized development with Docker
- Frontend-backend communication

---

## 🧠 How to Run (Backend)

### 🧩 Requirements
- [.NET 8 SDK or later](https://dotnet.microsoft.com/en-us/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Visual Studio 2022 or VS Code

### 🧰 Run Using .NET CLI
```bash
cd BookTrackerAPI
dotnet restore
dotnet build
dotnet run

🎯 Overview

The State Pattern is used in this project to manage a book’s reading status (Unread, Reading, or Read) in a more scalable and maintainable way.
Instead of using plain string values, each book now has a state object that determines how it transitions between statuses.

🧩 Why Use It

Previously, the Book model stored status as a simple string (e.g., "Reading"). Any time the status changed, logic for transitions (like setting “Read” or “Unread”) had to be handled manually in the controller or service layer.

By introducing the State Pattern, the Book model now contains its own behavior for changing states. This encapsulates logic within the model itself, making it easier to:

Add new states in the future (e.g., “Wishlist” or “Abandoned”)

Reduce duplicate code in controllers

Follow object-oriented best practices and the Single Responsibility Principle

🧱 Implementation Details

Each state (ReadState, UnreadState, ReadingState) implements the shared interface IBookState, which defines:

string name { get; }
void MarkAsRead(Book book);
void MarkAsUnread(Book book);
void MarkAsReading(Book book);


The Book model holds a reference to an IBookState:

public IBookState State { get; set; } = new ReadingState();


It provides helper methods to switch between states:

public void MarkAsRead() => State.MarkAsRead(this);
public void MarkAsUnread() => State.MarkAsUnread(this);
public void MarkAsReading() => State.MarkAsReading(this);


In the BooksController, when a user updates their reading status, the app now calls these helper methods:

switch (status)
{
    case "Read": userBook.Book.MarkAsRead(); break;
    case "Reading": userBook.Book.MarkAsReading(); break;
    case "Unread": userBook.Book.MarkAsUnread(); break;
}
userBook.Status = userBook.Book.State.name;

🪄 Result

The controller becomes simpler, and the model becomes smarter — each book knows how to manage its own state transitions.
This design keeps logic modular and prepares the codebase for future feature expansion (like reading progress, favorites, or shelves).
