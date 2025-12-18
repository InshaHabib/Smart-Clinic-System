using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using SmartClinic.Models.Entities;

namespace SmartClinic.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Appointment> RecentAppointments { get; set; }
    }

    public class CreateDoctorViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Specialty { get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        public string Qualifications { get; set; }

        [Required]
        [Display(Name = "Consultation Fee")]
        public decimal ConsultationFee { get; set; }

        [Display(Name = "Photo")]
        public HttpPostedFileBase Photo { get; set; }
    }

    public class EditDoctorViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        public string Specialty { get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        public string Qualifications { get; set; }

        [Required]
        [Display(Name = "Consultation Fee")]
        public decimal ConsultationFee { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Photo")]
        public HttpPostedFileBase Photo { get; set; }
        
        public string PhotoPath { get; set; }
    }

    public class ReportsViewModel
    {
        public decimal DailyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
    }
}