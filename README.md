# Clinic Management System

A comprehensive **Clinic Management System** built with **ASP.NET Core MVC**, **Entity Framework Core**, and **Identity** for managing appointments, doctors, and patients. This system supports multiple user roles with different access levels: **Admin**, **Doctor**, and **Patient**.



## Features

### Admin
- Dashboard showing all appointments and doctors.
- Assign or remove doctors to/from appointments.
- Add or remove doctors.
- Manage availability of doctors.
- Delete appointments as admin.

### Doctor
- Dashboard showing assigned appointments.
- Mark appointments as **Completed**.
- Add notes for patient appointments.

### Patient
- Dashboard showing personal appointments.
- Book, cancel, or delete appointments.
- View available doctors for new appointments.

### Authentication & Authorization
- Secure login and registration for Admin, Doctor, and Patient roles.
- Role-based access control using **ASP.NET Core Identity**.

### Email Notifications
- Reset password emails.
- Optional appointment notifications via email.



## Technology Stack

- **ASP.NET Core MVC**
- **Entity Framework Core** (Code First)
- **SQL Server / In-Memory Database for testing**
- **ASP.NET Core Identity** for user authentication
- **Moq & xUnit** for unit testing
- **Dependency Injection** for service layers


## Project Structure

```ClinicManagementSystem/
├── ClinicManagementSystem.UI/ # MVC Project (Controllers, Views)
├── ClinicManagementSystem.Core/ # Entities, DTOs, Interfaces, Enums
├── ClinicManagementSystem.Infrastructure/ # EF Core Services & Database Context
├── ClinicManagementSystem.ControllerTests/ # Unit tests for Controllers
├── ClinicManagementSystem.ServiceTests/ # Unit tests for Services
├── appsettings.json # Configuration file (not pushed with secrets)
└── README.md



## Setup Instructions

1. **Clone the repository**
```bash
git clone https://github.com/CoderShahzaib/Clinic-Management-System.git
cd ClinicManagementSystem
Restore NuGet packages

dotnet restore
Update connection string

Open appsettings.json (do not include sensitive info on GitHub).

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;"
}
Run Database Migrations

dotnet ef database update
Run the application

dotnet run
Access via browser

http://localhost:5000
Testing
Unit tests are written using xUnit and Moq.

Controller and service tests are available in ControllerTests and ServiceTests folders.

Run tests using:

dotnet test
