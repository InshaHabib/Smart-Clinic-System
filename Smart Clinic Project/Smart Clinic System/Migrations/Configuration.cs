using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartClinic.Models.Entities;
using SmartClinic.Models.Enums;

namespace SmartClinic.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SmartClinic.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SmartClinic.Data.ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            // Create Admin User
            if (!context.Users.Any(u => u.Email == "admin@smartclinic.com"))
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@smartclinic.com",
                    Email = "admin@smartclinic.com",
                    FullName = "System Admin",
                    Role = UserRole.Admin,
                    Phone = "03001234567",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };
                
                var result = userManager.Create(admin, "Admin@123");
                if (result.Succeeded)
                {
                    Console.WriteLine("Admin user created successfully");
                }
            }

            // Create Sample Doctor
            if (!context.Users.Any(u => u.Email == "doctor@smartclinic.com"))
            {
                var doctorUser = new ApplicationUser
                {
                    UserName = "doctor@smartclinic.com",
                    Email = "doctor@smartclinic.com",
                    FullName = "Dr. Ahmed Khan",
                    Role = UserRole.Doctor,
                    Phone = "03009876543",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };
                
                var result = userManager.Create(doctorUser, "Doctor@123");
                if (result.Succeeded)
                {
                    var doctor = new Doctor
                    {
                        UserId = doctorUser.Id,
                        Specialty = "Cardiologist",
                        Bio = "Experienced cardiologist with 10+ years",
                        Qualifications = "MBBS, FCPS (Cardiology)",
                        ConsultationFee = 2000,
                        IsAvailable = true
                    };
                    
                    context.Doctors.Add(doctor);
                    context.SaveChanges();
                    
                    // Add availability (Monday to Friday, 9 AM - 5 PM)
                    for (int i = 1; i <= 5; i++)
                    {
                        context.DoctorAvailabilities.Add(new DoctorAvailability
                        {
                            DoctorId = doctor.Id,
                            DayOfWeek = (DayOfWeek)i,
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(17, 0, 0),
                            IsAvailable = true
                        });
                    }
                    
                    Console.WriteLine("Doctor created successfully");
                }
            }

            // Create Sample Patient
            if (!context.Users.Any(u => u.Email == "patient@smartclinic.com"))
            {
                var patientUser = new ApplicationUser
                {
                    UserName = "patient@smartclinic.com",
                    Email = "patient@smartclinic.com",
                    FullName = "Ali Hassan",
                    Role = UserRole.Patient,
                    Phone = "03111234567",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };
                
                var result = userManager.Create(patientUser, "Patient@123");
                if (result.Succeeded)
                {
                    var patient = new Patient
                    {
                        UserId = patientUser.Id,
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Gender = "Male",
                        Address = "123 Main Street, Lahore",
                        BloodGroup = "O+",
                        EmergencyContact = "03001234567"
                    };
                    
                    context.Patients.Add(patient);
                    Console.WriteLine("Patient created successfully");
                }
            }

            // Add Sample Medicines
            if (!context.Medicines.Any())
            {
                context.Medicines.AddRange(new[]
                {
                    new Medicine { Name = "Panadol", Brand = "GSK", Description = "Pain reliever", UnitPrice = 50, QuantityInStock = 1000, ReorderLevel = 100, IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Augmentin", Brand = "GSK", Description = "Antibiotic", UnitPrice = 300, QuantityInStock = 500, ReorderLevel = 50, IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Brufen", Brand = "Abbott", Description = "Anti-inflammatory", UnitPrice = 80, QuantityInStock = 800, ReorderLevel = 100, IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Disprin", Brand = "Reckitt", Description = "Pain reliever", UnitPrice = 30, QuantityInStock = 1500, ReorderLevel = 200, IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Flagyl", Brand = "Sanofi", Description = "Antibiotic", UnitPrice = 150, QuantityInStock = 300, ReorderLevel = 50, IsActive = true, CreatedAt = DateTime.Now }
                });
                
                Console.WriteLine("Sample medicines added");
            }

            context.SaveChanges();
        }
    }
}