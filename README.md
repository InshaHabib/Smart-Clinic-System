# 🏥 Smart Clinic System

## 📋 Project Information

**Project Name:** Smart Clinic System  
**Technology Stack:** ASP.NET MVC 5, Entity Framework 6, SQL Server  
**Framework:** .NET Framework 4.7.2  
**Development Environment:** Visual Studio 2019/2022  
**Database:** SQL Server 2017+  

---

## 🎯 Project Overview

Smart Clinic is a comprehensive hospital management system designed to streamline healthcare operations with three distinct user roles: Admin, Doctor, and Patient. The system provides end-to-end management of appointments, medical records, prescriptions, billing, and inventory.

### Key Features

#### 🔐 Authentication & Authorization
- Role-based access control (Admin, Doctor, Patient)
- ASP.NET Identity for secure authentication
- Password hashing and encryption
- Email-based password recovery
- Session management

#### 👨‍💼 Admin Module
- **Dashboard:** Real-time statistics and KPIs
- **Doctor Management:** Add, edit, delete, and manage doctors
- **Patient Management:** View and manage patient records
- **Appointment Approval:** Approve/reject pending appointments
- **Inventory Management:** Medicine stock control with low-stock alerts
- **Billing Overview:** Track revenue and invoices
- **Reports & Analytics:** Daily/monthly reports

#### 👨‍⚕️ Doctor Module
- **Personal Dashboard:** Today's appointments and schedule
- **Appointment Management:** View patient appointments
- **Medical Records:** Add consultation notes and diagnosis
- **Prescription System:** Create digital prescriptions
- **Invoice Generation:** Generate patient invoices
- **Availability Settings:** Set working hours and days
- **Patient History:** Access complete patient medical history

#### 🧑‍🦱 Patient Module
- **Patient Dashboard:** Upcoming appointments and health overview
- **Doctor Directory:** Browse and search available doctors
- **Appointment Booking:** Book appointments with preferred doctors
- **Medical Records:** View personal medical history
- **Prescriptions:** Access and download prescription PDFs
- **Invoices:** View billing information and download invoices
- **Profile Management:** Update personal information

---

## 🛠️ Technology Stack

### Backend
- **Framework:** ASP.NET MVC 5 (.NET Framework 4.7.2)
- **ORM:** Entity Framework 6 (Code First)
- **Authentication:** ASP.NET Identity 2.2.3
- **Database:** SQL Server 2017+

### Frontend
- **UI Framework:** Bootstrap 5
- **JavaScript:** jQuery 3.6.0
- **CSS:** Custom CSS with Tailwind utility classes
- **Icons:** Font Awesome / Bootstrap Icons

### Additional Libraries
- **PDF Generation:** iTextSharp 5.5.13
- **Email Service:** System.Net.Mail (SMTP)
- **JSON Handling:** Newtonsoft.Json 13.0.1
- **Validation:** jQuery Validation
- **Security:** OWIN Authentication Middleware

---

## 💾 Database Schema

### Core Tables

1. **Users** - User accounts (Admin, Doctor, Patient)
2. **Doctors** - Doctor profiles and details
3. **Patients** - Patient information
4. **Appointments** - Appointment bookings
5. **DoctorAvailability** - Doctor schedules
6. **MedicalRecords** - Patient medical history
7. **Prescriptions** - Prescription headers
8. **PrescriptionItems** - Prescription details
9. **Medicines** - Medicine inventory
10. **Invoices** - Billing information

### Relationships

- User → Doctor (One-to-One)
- User → Patient (One-to-One)
- Doctor → Appointments (One-to-Many)
- Patient → Appointments (One-to-Many)
- Appointment → MedicalRecord (One-to-One)
- MedicalRecord → Prescriptions (One-to-Many)
- Prescription → PrescriptionItems (One-to-Many)
- Medicine → PrescriptionItems (One-to-Many)
- Appointment → Invoice (One-to-One)

---

## 🚀 Installation & Setup

### Prerequisites

- Visual Studio 2019 or 2022
- SQL Server 2017 or higher
- .NET Framework 4.7.2 or higher
- SQL Server Management Studio (optional)

### Step 1: Clone/Extract Project

```bash
# Extract the ZIP file to your desired location
# Or clone from repository
git clone https://github.com/InshaHabib/Smart-Clinic-System
cd SmartClinic
```

### Step 2: Open in Visual Studio

1. Open `SmartClinic.sln` in Visual Studio
2. Wait for NuGet packages to restore automatically
3. If packages don't restore, right-click solution → "Restore NuGet Packages"

### Step 3: Configure Database

Update connection string in `Web.config`:

```xml
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="Data Source=YOUR_SERVER;Initial Catalog=SmartClinicDB;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**For SQL Server Authentication:**
```xml
connectionString="Data Source=YOUR_SERVER;Initial Catalog=SmartClinicDB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD"
```

### Step 4: Run Migrations

Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)

```powershell
# Enable migrations (if not already enabled)
Enable-Migrations

# Create initial migration
Add-Migration InitialCreate

# Create database and seed data
Update-Database -Verbose
```

### Step 5: Configure Email (Optional)

Update SMTP settings in `Web.config`:

```xml
<appSettings>
  <add key="SmtpHost" value="smtp.gmail.com" />
  <add key="SmtpPort" value="587" />
  <add key="SmtpUser" value="your-email@gmail.com" />
  <add key="SmtpPass" value="your-app-password" />
</appSettings>
```

### Step 6: Build & Run

1. Build Solution: `Ctrl + Shift + B`
2. Run Project: `F5` or `Ctrl + F5`
3. Browser will open automatically

---

## 🎮 Usage Guide

### For Administrators

1. **Login:** Use admin credentials
2. **Dashboard:** View system statistics
3. **Manage Doctors:** Add/Edit/Delete doctors
4. **Approve Appointments:** Review and approve pending bookings
5. **Inventory:** Manage medicine stock
6. **Reports:** Generate revenue and activity reports

### For Doctors

1. **Login:** Use doctor credentials
2. **Dashboard:** View today's appointments
3. **Appointments:** Check scheduled patient visits
4. **Consultations:** Add medical records after examination
5. **Prescriptions:** Create and print prescriptions
6. **Invoices:** Generate patient bills
7. **Availability:** Set working hours

### For Patients

1. **Register:** Create new account
2. **Login:** Access patient portal
3. **Browse Doctors:** Search by specialty
4. **Book Appointment:** Select doctor and time slot
5. **View Records:** Access medical history
6. **Prescriptions:** Download prescription PDFs
7. **Invoices:** View and download bills

---

## 📊 Key Features Implementation

### Appointment Management
- Real-time availability checking
- Automatic conflict detection
- Email notifications on booking
- Status tracking (Pending/Approved/Completed/Cancelled)

### Medical Records
- Comprehensive patient history
- Vitals recording (BP, temperature, pulse)
- Diagnosis documentation
- Treatment notes

### Prescription System
- Digital prescription creation
- Medicine dosage and frequency
- PDF generation with clinic branding
- Automatic inventory deduction

### Billing & Invoicing
- Automated invoice generation
- Consultation fee calculation
- Medicine cost tracking
- Payment status management
- PDF invoice download

### Inventory Management
- Medicine stock tracking
- Low stock alerts
- Expiry date monitoring (future enhancement)
- Supplier information (future enhancement)

### Reports & Analytics
- Daily/Monthly revenue reports
- Appointment statistics
- Patient growth metrics
- Doctor performance indicators

---

## 📈 Future Enhancements

### Planned Features
- [ ] SMS notifications
- [ ] Online payment integration
- [ ] Telemedicine (video consultations)
- [ ] Lab test management
- [ ] Pharmacy module expansion
- [ ] Mobile app (React Native)
- [ ] Multi-language support
- [ ] Patient portal enhancements
- [ ] Insurance claim management
- [ ] Appointment reminders (WhatsApp)

### Technical Improvements
- [ ] Migrate to .NET Core/.NET 6+
- [ ] API development (RESTful)
- [ ] Microservices architecture
- [ ] Redis caching
- [ ] SignalR for real-time updates
- [ ] Unit test coverage
- [ ] Integration tests
- [ ] Performance optimization

---

## 🙏 Acknowledgments

- ASP.NET MVC Framework by Microsoft
- Entity Framework by Microsoft
- Bootstrap by Twitter
- jQuery by jQuery Foundation
- iTextSharp by iText Software

---

## 📊 Project Statistics

- **Lines of Code:** ~15,000+
- **Files:** 80+
- **Database Tables:** 15
- **Controllers:** 5
- **Models:** 20+
- **Services:** 7
- **Development Time:** 5 Days

---

## ✅ Project Status

**Current Version:** 1.0  
**Status:** ✅ Complete and Ready for Submission  
**Last Updated:** December 2025

---

**Built with ❤️ using ASP.NET MVC**

---

*This README was last updated on 18-12-2025*
