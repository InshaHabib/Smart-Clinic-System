public class MedicalRecord
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime VisitDate { get; set; }
    public string ChiefComplaint { get; set; }
    public string Diagnosis { get; set; }
    public string Vitals { get; set; } // JSON string
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public virtual Appointment Appointment { get; set; }
    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual ICollection<Prescription> Prescriptions { get; set; }
}