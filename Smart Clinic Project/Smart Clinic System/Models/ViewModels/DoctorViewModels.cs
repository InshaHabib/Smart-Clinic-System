using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SmartClinic.Models.Entities;

namespace SmartClinic.Models.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public Doctor Doctor { get; set; }
        public List<Appointment> TodayAppointments { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; }
        public int TotalPatients { get; set; }
    }

    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; }
        public List<MedicalRecord> PatientRecords { get; set; }
    }

    public class MedicalRecordViewModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "Visit Date")]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [Required]
        [Display(Name = "Chief Complaint")]
        [DataType(DataType.MultilineText)]
        public string ChiefComplaint { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Diagnosis { get; set; }

        [Display(Name = "Vitals (BP, Temp, etc.)")]
        [DataType(DataType.MultilineText)]
        public string Vitals { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public Appointment Appointment { get; set; }
    }

    public class PrescriptionViewModel
    {
        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }

        [Display(Name = "Special Instructions")]
        [DataType(DataType.MultilineText)]
        public string SpecialInstructions { get; set; }

        public List<PrescriptionItemViewModel> Items { get; set; }
        public List<Medicine> AvailableMedicines { get; set; }
    }

    public class PrescriptionItemViewModel
    {
        [Required]
        [Display(Name = "Medicine")]
        public int MedicineId { get; set; }

        [Required]
        [Display(Name = "Dosage (e.g., 500mg)")]
        public string Dosage { get; set; }

        [Required]
        [Display(Name = "Frequency (e.g., 3 times daily)")]
        public string Frequency { get; set; }

        [Required]
        [Display(Name = "Duration (days)")]
        public int DurationDays { get; set; }

        [DataType(DataType.MultilineText)]
        public string Instructions { get; set; }
    }

    public class InvoiceViewModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Consultation Fee")]
        public decimal ConsultationFee { get; set; }

        [Display(Name = "Medicine Cost")]
        public decimal MedicineCost { get; set; }

        public Appointment Appointment { get; set; }
    }
}