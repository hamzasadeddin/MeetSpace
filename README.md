# 🏢 MeetSpace — Meeting Room Booking Platform

![Platform](https://img.shields.io/badge/Platform-ASP.NET%20Core%209-512BD4?style=for-the-badge&logo=dotnet)
![Database](https://img.shields.io/badge/Database-SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/UI-Bootstrap%205-7952B3?style=for-the-badge&logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-22c55e?style=for-the-badge)

MeetSpace is a multi-tenant meeting room reservation platform built with ASP.NET Core 9 MVC and SQL Server. It enables companies to manage their own meeting rooms, book them with conflict detection, track availability in real time, and visualize all bookings through an interactive calendar dashboard — all wrapped in a modern, responsive UI.

---

## ✨ Features

- 🔐 **Session-based Authentication** — Secure login with no-cache headers and browser back-button protection after logout
- 🏢 **Multi-Company Isolation** — Each company sees and interacts only with its own rooms and bookings
- 📅 **Interactive Calendar** — FullCalendar v6 dashboard showing all bookings with click-to-view details and click-to-book on any date
- 🔄 **Recurring Bookings** — Daily, weekly, or monthly recurrence with a configurable end date
- ⚡ **Live Availability** — Room statuses update in real time based on current bookings at the moment of viewing
- 🚫 **Conflict Detection** — Prevents double-booking across all time slots including recurring ones
- 🏛️ **Room Management** — Users can create, edit, and soft-delete rooms scoped to their company
- 📋 **My Bookings** — Personal booking history with live status tracking (Confirmed, In Room, Finished, Cancelled)
- 🔒 **Cancel Protection** — Bookings that have already started cannot be cancelled
- 👤 **Auto Organizer** — Booking form pre-fills the logged-in user's name and locks the field
- 🎨 **Modern UI** — Glassmorphism-inspired light theme with animations, custom fonts, and fully responsive layout

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 9 MVC |
| Language | C# 13 |
| ORM | Entity Framework Core 9 |
| Database | Microsoft SQL Server |
| Frontend | Bootstrap 5.3, Font Awesome 6.5 |
| Calendar | FullCalendar v6 |
| Fonts | Clash Display, Nunito (Google Fonts) |
| Auth | Session-based (custom, no Identity) |

---

## 🗄️ Database Schema

### `MeetingRoom`
| Column | Type | Description |
|---|---|---|
| `Room_Id` | INT (PK) | Auto-increment primary key |
| `Room_Name` | NVARCHAR(100) | Display name of the room |
| `Room_Location` | NVARCHAR(255) | Physical location / floor |
| `Room_Capacity` | INT | Maximum number of people |
| `Room_Amenities` | NVARCHAR(500) | Comma-separated list of amenities |
| `Room_Company` | NVARCHAR(100) | Company the room belongs to |
| `Room_IsActive` | TINYINT | Soft delete flag (1 = active) |

### `AppUser`
| Column | Type | Description |
|---|---|---|
| `User_Id` | INT (PK) | Auto-increment primary key |
| `User_FName` | NVARCHAR(100) | First name |
| `User_LName` | NVARCHAR(100) | Last name |
| `User_Logon` | NVARCHAR(255) | Email address (unique) |
| `User_Password` | NVARCHAR(255) | Password |
| `User_Company` | NVARCHAR(100) | Company the user belongs to |
| `User_IsActive` | TINYINT | Active flag (1 = active) |

### `Communication` (Bookings)
| Column | Type | Description |
|---|---|---|
| `Comm_CommunicationId` | INT (PK) | Auto-increment primary key |
| `Comm_Subject` | NVARCHAR(255) | Meeting title |
| `Comm_Organizer` | NVARCHAR(384) | Name of the person who booked |
| `Comm_DateTime` | DATETIME | Booking start date and time |
| `Comm_ToDateTime` | DATETIME | Booking end date and time |
| `Comm_MeetingRoom` | NVARCHAR(40) | Name of the booked room |
| `Comm_Status` | NVARCHAR(40) | Confirmed / Cancelled |
| `Comm_RecurrenceRule` | NVARCHAR(500) | daily / weekly / monthly |
| `Comm_Deleted` | TINYINT | Soft delete flag |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or full)
- [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.8+) or VS Code

---

### 1. Clone the repository
```bash
git clone https://github.com/your-username/meetspace.git
cd meetspace
```

---

### 2. Set up the database

Open SSMS and run the following scripts in order:

**Create the database:**
```sql
CREATE DATABASE MeetingRoomDB;
GO
```

**Run the table creation scripts** from the `/Database` folder in this repository (or copy from the scripts below):
- `01_Communication.sql`
- `02_MeetingRoom.sql`
- `03_AppUser.sql`
- `04_SeedData.sql`

---

### 3. Configure the connection string

Open `appsettings.json` and update the connection string to match your SQL Server instance:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MeetingRoomDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> If you are using SQL Server Express, change `Server=localhost` to `Server=localhost\SQLEXPRESS`

---

### 4. Install dependencies
```bash
dotnet restore
```

---

### 5. Run the project
```bash
dotnet run
```

Then open your browser at `https://localhost:5001`


---

## 🏗️ Project Structure
MeetingRoomBooking/
├── Controllers/
│   ├── AuthController.cs        — Login / Logout
│   ├── HomeController.cs        — Dashboard + Calendar
│   ├── BookingController.cs     — Room listing, Create, My Bookings, Cancel
│   └── RoomController.cs        — Manage Rooms (Create, Edit, Delete)
├── Data/
│   └── AppDbContext.cs          — Entity Framework DbContext
├── Filters/
│   ├── NoCacheAttribute.cs      — Prevents browser caching of protected pages
│   └── RequireLoginAttribute.cs — Session guard on all protected controllers
├── Models/
│   ├── AppUser.cs
│   ├── MeetingRoom.cs
│   ├── Communication.cs
│   ├── BookingViewModel.cs
│   ├── RoomViewModel.cs
│   └── LoginViewModel.cs
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml
│   ├── Booking/
│   │   ├── Index.cshtml         — Room listing with live availability
│   │   ├── Create.cshtml        — Booking form with recurring support
│   │   └── MyBookings.cshtml    — Personal booking history
│   ├── Home/
│   │   └── Index.cshtml         — Hero + FullCalendar dashboard
│   ├── Room/
│   │   ├── Index.cshtml         — Manage rooms
│   │   ├── Create.cshtml        — Add new room
│   │   └── Edit.cshtml          — Edit existing room
│   └── Shared/
│       └── _Layout.cshtml       — Navbar, logout modal, footer
└── wwwroot/
├── css/site.css
├── js/site.js
└── images/
└── RoomoraLogo.png

---

## 🔒 Security Notes

- All protected controllers are decorated with `[RequireLogin]` and `[NoCache]` filters
- On logout, the session is cleared, the session cookie is deleted, and `no-store` cache headers are applied — pressing the browser back button redirects to the login page
- The organizer field on booking forms is pre-filled from the session and cannot be modified by the user
- Room and booking operations are scoped to the logged-in user's company — cross-company access is blocked at the controller level

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

Built with ❤️ using ASP.NET Core 9, Entity Framework, and FullCalendar.
