# 🏢 MeetSpace — Meeting Room Booking Platform
 
![Platform](https://img.shields.io/badge/Platform-ASP.NET%20Core%209-512BD4?style=for-the-badge&logo=dotnet)
![Database](https://img.shields.io/badge/Database-SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/UI-Bootstrap%205-7952B3?style=for-the-badge&logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-22c55e?style=for-the-badge)
 
MeetSpace is a multi-tenant meeting room reservation platform built for **Alayyan Group** using ASP.NET Core 9 MVC and SQL Server. It enables companies across the group to manage their own meeting rooms, book them with real-time conflict detection, track live availability, and visualize all bookings through an interactive calendar dashboard — all wrapped in a refined navy and gold UI that reflects the Alayyan brand identity.
 
---
 
## ✨ Features
 
### 🔐 Authentication & Security
- Session-based login with role support (`Admin` / `User`)
- No-cache headers and browser back-button protection after logout
- Logout confirmation modal before session is cleared
- All protected pages decorated with `[RequireLogin]` and `[NoCache]` filters
 
### 🏢 Multi-Company Isolation
- Six supported companies: **Kia Jordan**, **Kia Iraq**, **GAC Jordan**, **GAC Iraq**, **BYD Iraq**, **Isuzu**
- Each user sees and interacts only with rooms belonging to their own company
- Room and booking operations are scoped at the controller level — cross-company access is blocked
 
### 👑 Admin Role
- Admin users can view, create, edit, and remove rooms across **all companies**
- Non-admin users cannot access the Manage Rooms section — the nav link is hidden and the route is protected
- Company is selectable by admins when creating or editing a room
 
### 📅 Interactive Calendar Dashboard
- FullCalendar v6 integration on the home page showing all bookings for the user's company
- Events display the **room name** (not the meeting subject — privacy by design)
- Click any event to view full booking details including organizer contact info
- Click any empty date to open a **Quick Book** modal pre-filled with that date
- Color-coded legend per room with matching event colors
- Start and end time shown on each event chip in month view
- Week and Day views available for detailed scheduling
 
### ⚡ Live Room Availability
- Room statuses update in real time based on current bookings at the moment of viewing
- Rooms page shows **Available** or **Occupied** with a pulsing live indicator
- Occupied rooms still allow future bookings via a **Book Later** button
- Available rooms counter on the home page reflects the current moment
 
### 📋 Room Booking
- Intuitive room selector with capacity and amenity information
- Organizer name pre-filled from session and locked — cannot be edited
- Time picker limited to **HH:mm** — no seconds
- Required and Optional **invitees** fields (comma-separated emails)
- Required invitees field is mandatory — enforced both client-side and server-side
 
### 🔄 Recurring Bookings
- Daily, weekly, or monthly recurrence patterns
- Maximum recurrence span of **one quarter (92 days)** per booking series
- If a longer schedule is needed, users are directed to open a support ticket
- Client-side validation shows the maximum allowed end date instantly
 
### 🚫 Conflict Detection & Time Slots
- Real-time conflict checking before submission — no round trip needed
- Conflict banner shows exactly which time slot is taken and when the room is next free
- **Available Slots panel** shows free and busy windows for the selected room and date
- Slots start from **07:30** and end at **17:30**
- Past slots are never shown — even if the room was free, past times cannot be booked
- Clicking a free slot auto-fills the start and end time inputs
 
### 📊 My Bookings
- Shows only the logged-in user's own bookings (privacy-scoped)
- Live status computed at request time: **Confirmed**, **In Room**, **Finished**, **Cancelled**
- Bookings that have already started cannot be cancelled
- Duration column shows the length of each meeting
- Invitees shown in a styled hover popup with Required and Optional sections
 
### 🔄 Recurring Series Management
- Recurring bookings show a **Manage Series** button instead of a simple Cancel
- Three cancellation options presented in a styled modal:
  - **Cancel This Session Only** — removes one occurrence
  - **Cancel This & All Future Sessions** — trims the series from this point forward
  - **Cancel All Upcoming Sessions** — wipes the entire future schedule
- Past sessions are never touched by any cancellation action
 
### 👤 Organizer Contact Details
- Clicking a booking in the calendar shows the organizer's **phone**, **email**, and **department**
- Details are retrieved automatically from the `AppUser` table — no manual entry needed
 
### 🏛️ Room Management (Admin Only)
- Create, edit, and soft-delete rooms
- Quick-add amenity chips for common items (Projector, Whiteboard, etc.)
- Company badge shown on each room card so admins can identify ownership at a glance
- Soft delete preserves booking history
 
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
| Fonts | Cormorant Garamond, DM Sans (Google Fonts) |
| Auth | Session-based (custom, no ASP.NET Identity) |
 
---
 
## 🗄️ Database Schema
 
### `AppUser`
| Column | Type | Description |
|---|---|---|
| `User_Id` | INT (PK) | Auto-increment primary key |
| `User_FName` | NVARCHAR(100) | First name |
| `User_LName` | NVARCHAR(100) | Last name |
| `User_Logon` | NVARCHAR(255) | Email address (unique) |
| `User_Password` | NVARCHAR(255) | Password |
| `User_Company` | NVARCHAR(100) | Company the user belongs to |
| `User_Role` | NVARCHAR(20) | `Admin` or `User` |
| `User_Phone` | NVARCHAR(30) | Contact phone number |
| `User_Email` | NVARCHAR(255) | Contact email address |
| `User_Department` | NVARCHAR(100) | Department within the company |
| `User_IsActive` | TINYINT | Active flag (1 = active) |
 
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
 
### `Communication` (Bookings)
| Column | Type | Description |
|---|---|---|
| `Comm_CommunicationId` | INT (PK) | Auto-increment primary key |
| `Comm_Subject` | NVARCHAR(255) | Meeting title |
| `Comm_Organizer` | NVARCHAR(384) | Full name of the person who booked |
| `Comm_DateTime` | DATETIME | Booking start date and time |
| `Comm_ToDateTime` | DATETIME | Booking end date and time |
| `Comm_MeetingRoom` | NVARCHAR(40) | Name of the booked room |
| `Comm_Status` | NVARCHAR(40) | `Confirmed` or `Cancelled` |
| `Comm_Description` | NVARCHAR(MAX) | Optional meeting notes |
| `Comm_Location` | NVARCHAR(255) | Physical location override |
| `Comm_RecurrenceRule` | NVARCHAR(500) | `daily` / `weekly` / `monthly` |
| `Comm_RecurrenceGroupId` | NVARCHAR(40) | Groups all sessions of a recurring series |
| `Comm_RequiredInvitees` | NVARCHAR(MAX) | Comma-separated required attendee emails |
| `Comm_OptionalInvitees` | NVARCHAR(MAX) | Comma-separated optional attendee emails |
| `Comm_Deleted` | TINYINT | Soft delete flag |
| `Comm_CreatedDate` | DATETIME | Record creation timestamp |
| `Comm_UpdatedDate` | DATETIME | Last update timestamp |
 
---
 
## 🚀 Getting Started
 
### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or full)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) v17.8 or later
 
---
 
### 1. Clone the repository
 
```bash
git clone https://github.com/your-username/meetspace.git
cd meetspace
```
 
---
 
### 2. Set up the database
 
Open SSMS and run the following in order:
 
**Create the database:**
```sql
CREATE DATABASE MeetingRoomDB;
GO
```
 
**Run the table creation and seed scripts** from the `/Database` folder in this repository:
- `01_Communication.sql`
- `02_MeetingRoom.sql`
- `03_AppUser.sql`
- `04_SeedData.sql`
 
---
 
### 3. Configure the connection string
 
Open `appsettings.json` and update to match your SQL Server instance:
 
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MeetingRoomDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
 
> If using SQL Server Express: `Server=localhost\SQLEXPRESS`
 
---
 
### 4. Install dependencies and run
 
```bash
dotnet restore
dotnet run
```
 
Then open your browser at `https://localhost:5001`
 
---
 
## 🏗️ Project Structure
 
```
MeetingRoomBooking/
├── Constants/
│   ├── SessionKeys.cs           — Session key name constants
│   └── BookingStatus.cs         — Status and type string constants
├── Controllers/
│   ├── AuthController.cs        — Login / Logout
│   ├── HomeController.cs        — Dashboard + Calendar
│   ├── BookingController.cs     — Rooms, Create, My Bookings, Cancel, Series, Slots
│   └── RoomController.cs        — Admin room management (Create, Edit, Delete)
├── Data/
│   └── AppDbContext.cs          — Entity Framework DbContext
├── Filters/
│   ├── NoCacheAttribute.cs      — Prevents browser caching of protected pages
│   ├── RequireLoginAttribute.cs — Session guard on all protected controllers
│   └── RequireAdminAttribute.cs — Admin-only route guard
├── Models/
│   ├── AppUser.cs               — User entity with role, dept, contact fields
│   ├── MeetingRoom.cs           — Room entity with company field
│   ├── Communication.cs         — Booking entity (maps to legacy CRM table)
│   ├── BookingViewModel.cs      — Form model for creating bookings
│   ├── RoomViewModel.cs         — Form model for creating/editing rooms
│   └── LoginViewModel.cs        — Login form model
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml         — Branded login page (no layout)
│   ├── Booking/
│   │   ├── Index.cshtml         — Room listing with live availability
│   │   ├── Create.cshtml        — Booking form with slot picker and recurrence
│   │   └── MyBookings.cshtml    — Personal booking history with series management
│   ├── Home/
│   │   └── Index.cshtml         — Hero section + FullCalendar dashboard
│   ├── Room/
│   │   ├── Index.cshtml         — Admin room listing
│   │   ├── Create.cshtml        — Add new room form
│   │   └── Edit.cshtml          — Edit existing room form
│   └── Shared/
│       └── _Layout.cshtml       — Navbar, logout modal, footer with Alayyan branding
└── wwwroot/
    ├── css/site.css             — Full custom CSS (navy/gold Alayyan theme)
    └── js/site.js
```
 
---
 
## 🔒 Security
 
- All protected controllers carry `[RequireLogin]` and `[NoCache]`
- On logout: session cleared, session cookie deleted, `no-store` headers applied — back button redirects to login
- Organizer field locked to logged-in user — cannot be spoofed via form submission
- Room and booking operations validated against the user's company server-side
- Admin-only routes protected by `[RequireAdmin]` — redirect to Home if accessed by regular users
- Recurring series cancellation verifies organizer ownership before any modification
 
---
 
## 🏢 Supported Companies & Room Assignment
 
| Company | Rooms |
|---|---|
| Kia Jordan | Boardroom Alpha, Collaboration Hub |
| Kia Iraq | Innovation Lab |
| GAC Jordan | Executive Suite |
| GAC Iraq | Focus Room A |
| BYD Iraq | Focus Room B |
| Isuzu | *(add via Manage Rooms as Admin)* |
 
---
 
## 📋 Business Rules
 
| Rule | Detail |
|---|---|
| Booking window | 07:30 — 17:30 daily |
| Max recurrence | 92 days (one quarter) per series |
| Past slots | Cannot be booked or displayed — filtered in real time |
| Subject privacy | Meeting subject visible only to the organizer in My Bookings |
| Calendar display | Shows room name only — subject never shown publicly |
| Series cancellation | Past sessions are never affected — only future ones |
| Required invitees | Mandatory field — booking cannot be submitted without at least one |
 
---
 
## 📄 License
 
This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
 
---
 
## 👨‍💻 Built With
 
ASP.NET Core 9 · Entity Framework Core · FullCalendar v6 · Bootstrap 5 · SQL Server
 
Developed for **Alayyan Group** — Since 1967
