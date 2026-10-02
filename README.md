# Student ERP

A full-stack **Student ERP** web application built with **ASP.NET Core (8.0)** that manages the day-to-day academic and administrative operations of a school/college. It covers student registration, admission, class/section & subject management, fee structure setup, fee payments, PDF receipt generation and automatic email delivery.

The backend is a REST API written in C# using **Dapper** over **SQL Server**, while the front end is a set of lightweight **HTML + CSS + JavaScript + Bootstrap** pages served from `wwwroot` that talk to the API with `fetch`.

---

## Features

### Admin Section
- Manage **Classes / Sections** and view existing class sections
- Manage **Subjects** and assign subjects to a class section
- Manage **Fee Types** (create / delete)
- **Class Fee Mapping** – map fee types to a particular class section
- **Fee Structure** – define the fee amount per fee type / standard

### Administration (Admission) Section
- Add / edit **Students** (with profile image upload)
- Search students and view student details with contacts
- **Student Admission** into a class section with a custom discount option
- View and **update fee details** for already admitted students
- **Fee Payment** – view fee balance and pay/update the balance
- **PDF receipt generation** (QuestPDF) for each payment
- **Automatic email** of the receipt to the student & parent (MailKit / SMTP)

### Authentication
- Login API that issues a **JWT** stored in an **HttpOnly cookie**
- JWT bearer authentication middleware validates the token on protected routes
- Role claim included in the token for role-based access

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8.0 (Web API + MVC) |
| Language | C# |
| Data Access | Dapper 2.1 + `Microsoft.Data.SqlClient` |
| Database | Microsoft SQL Server (tables + stored procedures) |
| Auth | JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| PDF | QuestPDF |
| Email | MailKit / MimeKit (SMTP) |
| Logging | Serilog (Console + rolling file) |
| API Docs | Swashbuckle (Swagger / Swagger UI) |
| Frontend | HTML, CSS, JavaScript (fetch API), Bootstrap 5, Boxicons |

---

## Architecture

```
Browser (HTML/CSS/JS in wwwroot)
        │  fetch()  →  JSON
        ▼
Controllers  (api/auth, api/admin, api/adm)   ← thin, only validate + return
        ▼
Interfaces (IAuthService, IAdminSectionService, IAdministrationService, IEmailService)
        ▼
Services  (business logic + Dapper calls + stored procedures)
        ▼
SQL Server  (tables, joins and stored procedures)
```

- **Controllers** – receive HTTP requests, call the matching service and return a standard response DTO.
- **Interfaces / Services** – service abstractions and their implementations containing all business logic and DB access.
- **DTOs** – request/response contracts (e.g. `LoginRequestDTO`, `StudentDTO`, `*ResponseDTO`).
- **Models** – entities returned from the database (e.g. `Student`, `FeeType`, `ClassModel`).

---

## Project Structure

```
Project_StudentERP/
├── Controllers/            # API endpoints
│   ├── AuthController.cs           # /api/auth  – login
│   ├── AdminSectionController.cs   # /api/admin – classes, subjects, fee setup
│   ├── AdministrationController.cs # /api/adm   – students, admission, fees, receipts
│   └── HomeController.cs           # MVC views
├── Interfaces/            # Service contracts
├── Services/              # Service implementations (Dapper + stored procedures)
├── DTOs/                  # Request / response objects
│   └── DTOs_new/
├── Models/                # Database entities
├── Documents/
│   ├── ReceiptDocument.cs          # QuestPDF fee receipt layout
│   └── EmailTemplates/             # EmailSettings + PaymentReceipt.html
├── Views/                 # Razor MVC views (Home, Shared)
├── wwwroot/               # Frontend (static files)
│   ├── index.html                  # Login page
│   ├── dashboard.html              # Main dashboard
│   ├── Admin/                      # Admin section pages + JS
│   ├── Administration/             # Admission / Fee pages + JS
│   ├── css/, js/, lib/             # Site assets & Bootstrap/jQuery libs
│   └── studentImages/              # Uploaded student profile images
├── logs/                  # Serilog rolling log files
├── Program.cs             # App startup, DI, JWT & middleware config
└── appsettings.json       # Connection string + config
```
---

## API Endpoints

### Authentication — `/api/auth`
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Validate credentials, set `accessToken` cookie |

### Admin Section — `/api/admin`
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/add/class` | Add a class |
| GET | `/get/classes` | Get all classes |
| DELETE | `/del/class/{id}` | Delete a class |
| GET | `/get/sections` | Get all sections |
| GET | `/get/standards` | Get all standards |
| POST | `/add/subject` | Add a subject |
| GET | `/get/subjects/{csid?}` | Get all / class-section subjects |
| DELETE | `/del/subject/{id}` | Delete a subject |
| POST | `/add/subjectClass` | Assign subjects to a class section |
| POST | `/add/feeType` | Add a fee type |
| GET | `/get/feeType/{csid?}` | Get all / class-section fee types |
| DELETE | `/del/feeType/{id}` | Delete a fee type |
| GET | `/get/feeTypes/StdId/{id}` | Fee types for a standard |
| POST | `/add/classSectionFeeMap` | Map fee types to a class section |
| POST | `/add/feeStructure` | Save fee structure amounts |

### Administration — `/api/adm`
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/addStudent` | Add / edit a student (multipart form) |
| GET | `/students` | Get all students |
| GET | `/student/{id}` | Get student by id |
| POST | `/student/search` | Search students |
| GET | `/student/contacts/{id}` | Student contacts |
| GET | `/get/classSectionFeeType/{id}` | Fee types for a class section |
| POST | `/student/addmission` | Admit a student (with discount) |
| GET | `/get/fee/student/{id}` | Fee info of an admitted student |
| POST | `/update/feeDetails/student` | Update admitted student fee details |
| GET | `/get/balanceFee/student/{id}` | Fee balance of a student |
| POST | `/update/balance/fee/student` | Pay / update the balance |
| GET | `/download/receipt/{receiptId}` | Download PDF receipt |
| GET | `/get/receipts/student/{id}` | All receipts of a student |

> Swagger UI is available at `/swagger` when the app is running.
---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (Express/Developer) with the `StudentERP_Project` database
- A Gmail account (or any SMTP server) with an app password for email receipts

### 1. Clone the repository
```bash
git clone https://github.com/sarthak0401/StudentERP.git
cd StudentERP
```

### 2. Configure the application
Set the database connection in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=StudentERP_Project;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Provide the required secrets (do **not** commit these). Use **User Secrets** in development:
```bash
dotnet user-secrets set "Jwt:Key" "your-very-long-secret-signing-key"
dotnet user-secrets set "EmailSettings:Email" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```
`EmailSettings:Host` and `Port` are already set in `appsettings.json` (Gmail SMTP, port 587).

### 3. Set up the database
Create the `StudentERP_Project` database and the required tables / stored procedures
(see the list below). The application does not create the schema automatically.

### 4. Run the application
```bash
dotnet restore
dotnet run --launch-profile https
```
Then open **https://localhost:7098** in your browser, log in, and you'll land on the dashboard.

> Ports (from `launchSettings.json`): **https://localhost:7098**, http `http://localhost:5182`.

---

## Configuration Reference

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Jwt:Key` | HMAC-SHA256 signing key for JWT (store in user secrets) |
| `EmailSettings:Host` / `Port` | SMTP server (default `smtp.gmail.com:587`) |
| `EmailSettings:Email` / `Password` | SMTP credentials (store in user secrets) |

The JWT is written to the `accessToken` cookie (`HttpOnly`, `Secure`, `SameSite=Strict`, 15-minute expiry) and read back by the JWT bearer middleware.

---

## Database Objects

**Main tables used:** `UserLogin`, `StudentDetails`, `StudentContact`, `Classes`, `ClassSectionFeeType`, `SectionMaster`, `Standards` (plus subjects / fee-type tables).

**Stored procedures used:**

| Stored Procedure | Used for |
|------------------|----------|
| `sp_getUserById` | Lookup a user |
| `sp_searchStudent` | Student search |
| `sp_addOrEditStudent` | Add / edit student |
| `sp_studentAdmission` | Admit a student |
| `sp_getAllSelectedFeeTypesForParticularStudent` | Fee details of a student |
| `sp_updateAdmittedStudentFee` | Update admitted student fees |
| `sp_getFeeBalanceForAdmittedStudent` | Student fee balance |
| `sp_updateBalanceFeeAmtForAdmittedStudent` | Update / pay balance |
| `sp_getAllFeeTypesForParticularClassSection` | Fee types of class section |
| `sp_GetReceipt` | Generate receipt data |
| `sp_getAllReceiptsForAParticularStudent` | List student receipts |
| `sp_addOrEditFeeType` | Add / edit fee type |
| `sp_deleteFeeType` | Delete fee type |
| `sp_getAllFeeTypesSelectedForAParticularClsSection` | Fee types mapped to class section |
| `sp_addClassSectionFeeTypes` | Map fee types to class section |
| `sp_getFeeTypesForParticularStd` | Fee types for a standard |
| `sp_addStdFeeTypes` | Map fee types to standard |
| `sp_SaveFeeStructure` | Save fee structure |
| `sp_addClass` | Add class |
| `sp_getAllClasses` | List classes |
| `sp_addSubject` | Add subject |
| `sp_getAllSubjects` | List subjects |
| `sp_deleteSubject` | Delete subject |
| `sp_AssignSubjectstToClassSection` | Assign subjects to class section |

---

## Notes

- The frontend pages currently **hardcode** the API base URL as `https://localhost:7098`. If you run on a different port, update the `fetch` URLs in the JS files under `wwwroot/Admin`, `wwwroot/Administration/Js`, and `wwwroot/index.html`.
- Student profile images are stored in `wwwroot/studentImages`.
- Serilog writes to the console and to daily rolling files in `logs/app-*.txt`.
- QuestPDF is used under the free **Community license**.
- Swagger is enabled for exploring and testing the APIs.