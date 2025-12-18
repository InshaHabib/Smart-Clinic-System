using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SmartClinic.Models.Entities;

namespace SmartClinic.Models.ViewModels
{
    public class PatientDashboardViewModel
    {
        public Patient Patient { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; }
        public List<Prescription> RecentPrescriptions { get; set; }
        public List<Invoice> PendingInvoices { get; set; }
    }

    public class BookAppointmentViewModel
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        [Display(Name = "Appointment Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Reason for Visit")]
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public List<DoctorAvailability> DoctorAvailability { get; set; }
    }
}