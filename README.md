# Personal Expense Tracker

A simple single-user expense tracking application built to practice modern **.NET** and **Blazor** development.

## Features

The application allows users to:

- Add, edit, and delete expenses
- Search, filter, and sort expenses
- Filter expenses by category and date
- View dashboard statistics
- Register and authenticate users
- Access protected pages with authorization

## Tech Stack

Built using:

- **ASP.NET Core**
- **Blazor Web App**
- **C#**
- **Entity Framework Core**
- **MySQL**
- **ASP.NET Core Identity**
- **Bootstrap**

## Architecture

The application follows a simple layered architecture:

- **Blazor Components** handle the user interface and interactions
- **Services** manage application logic and business rules
- **Entity Framework Core** manages database communication
- **MySQL** stores application data

## Project Structure

```text
Components/     - Blazor UI components and pages
Data/           - Database context and configuration
Models/         - Application entities
Services/       - Business logic services
Migrations/     - EF Core database migrations
```

## Database

The project uses **Entity Framework Core migrations** for database management.

### Update Database

```bash
dotnet ef database update
```

### Create a Migration

```bash
dotnet ef migrations add MigrationName
```

## Running the Application

### Install Dependencies

```bash
dotnet restore
```

### Run the Application

```bash
dotnet run
```

### Verify the Build

```bash
dotnet build
```

## Learning Goals

This project demonstrates working with:

- Blazor components
- Routing
- Dependency injection
- Forms and validation
- Entity Framework Core
- LINQ
- Authentication and authorization

---

Built as a practice project to explore modern **.NET application development with Blazor**.
