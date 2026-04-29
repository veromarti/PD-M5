# Sport Reservation System

A web-based management system built with ASP.NET Core MVC for handling users, sport spaces, and reservations in a sport complex. Built with EF Core, LINQ, MySQL, and SMTP email notifications.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MySQL Server running locally
- `dotnet-ef` installed globally:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## Getting Started

### 1. Clone the repository: https://github.com/veromarti/PD-M5

```bash
git clone https://github.com/veromarti/PD-M5.git
cd PD-M5
```

### 2. Create the `appsettings.json` file

> This file is **not included in the repository** (excluded via `.gitignore` for security). You must create it manually inside the `SportComplex/` folder.

Create `SportComplex/appsettings.json` with the following content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=sportreservation;user=root;password=YOUR_PASSWORD_HERE"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "User": "your_email@gmail.com",
    "Password": "your_app_password"
  }
}
```

> If you use Gmail, generate an **App Password** from your Google account (Security → 2-Step Verification → App Passwords). Email notifications are optional — the system works without them.

### 3. Restore packages

```bash
dotnet restore
```

### 4. Apply migrations and create the database

Migrations are already included in the repository. Just run:

```bash
dotnet ef database update
```

This will automatically create the `users`, `sport_spaces`, and `reservations` tables in MySQL.


### 5. Run the project

```bash
dotnet run
```

Open your browser at: **`http://localhost:5167`**

---

## Project Structure

```
SportComplex/
├── Controllers/
│   ├── HomeController.cs           → Dashboard with summary stats
│   ├── UserController.cs           → User management (list, create, edit, delete)
│   ├── SportSpaceController.cs     → Sport space management
│   └── ReservationController.cs    → Booking management with business validations
├── Data/
│   └── MySqlDbContext.cs           → EF Core database context (MySQL)
├── Migrations/                     → EF Core generated migrations
├── Models/
│   ├── User.cs                     → User entity (name, document, phone, email)
│   ├── SportSpace.cs               → Sport space entity (name, type, capacity)
│   ├── Reservation.cs              → Booking entity (user, space, date, time, status)
│   └── ErrorViewModel.cs
├── Responses/
│   └── ServiceResponse.cs          → Generic service response wrapper
├── Services/
│   ├── UserService.cs              → User business logic (CRUD + validations)
│   ├── SportSpaceService.cs        → Sport space business logic (CRUD + filter by type)
│   ├── ReservationService.cs       → Booking logic + overlap validations + email notifications
│   └── EmailService.cs             → SMTP email sending
├── Views/
│   ├── Home/                       → Dashboard
│   ├── User/                       → Index, Create, Edit, Show
│   ├── SportSpace/                 → Index, Create, Edit, Show
│   ├── Reservation/                → Index, Create, Edit, Show
│   └── Shared/                     → _Layout, Error
├── wwwroot/
├── Program.cs
├── appsettings.json                → Not included — create manually (see step 2)
└── SportComplex.csproj
```

---

## Features

### Users
- Register users with name, ID number, phone, and email
- Edit existing user information
- Validate uniqueness by ID number and email (no duplicates allowed)
- List all registered users
- Delete users

### Sport Spaces
- Register sport spaces with name, type (Soccer, Basketball, Swimming Pool, Tennis, Volleyball, Squash, Athletics, Other), and capacity
- Edit sport space information
- Validate that no two spaces share the same name
- List all spaces and filter by type
- Delete sport spaces

### Bookings
- Create a booking linking a user, a sport space, a date, a start time, and an end time
- Validate that no overlapping bookings exist for the same sport space
- Validate that the same user does not have two bookings in the same time range
- Validate that the end time is after the start time
- Validate that bookings cannot be created in the past
- Manage booking statuses: **Active**, **Cancelled**, **Finished**
- Cancel a booking (changes its status to "Cancelled")
- Filter bookings by user or by sport space
- Delete bookings

### Email Notifications
- A confirmation email is sent to the user when a booking is successfully created
- A notification email is sent when a booking is cancelled

---

## Business Rules

| Rule | Description |
|------|-------------|
| No space overlap | A sport space cannot have two active bookings in the same date and time range |
| No user overlap | A user cannot have two active bookings in the same date and time range |
| End time > start time | Validated before saving |
| No past bookings | Bookings cannot be created on past dates or past times on the current day |
| Unique ID number | No two users can share the same ID number |
| Unique email | No two users can share the same email address |
| Unique space name | No two sport spaces can share the same name |

---

## Tech Stack

| Technology | Purpose |
|------------|---------|
| ASP.NET Core MVC (.NET 10) | Web framework |
| Entity Framework Core 9 + Pomelo.EntityFrameworkCore.MySql | ORM and database access |
| LINQ | Data queries and overlap validations |
| `List<T>` | Collections in services and views |
| MySQL | Relational database |
| SMTP / System.Net.Mail | Email notifications |
| MDBootstrap 7 + Font Awesome 6 | UI styling |

---

## Error Handling

- All controllers implement `try-catch` blocks that catch unexpected exceptions and display clear messages to the user without exposing technical details.
- Services return a `ServiceResponse<T>` containing `Success`, `Message`, and `Data`, allowing controllers to distinguish between business rule violations and system errors.
- Business validations are applied before any write operation to the database.

---

## Diagrams

See the `/docs` folder for:
- `class-diagram.png` — Class diagram
- `use-case-diagram.png` — Use case diagram
