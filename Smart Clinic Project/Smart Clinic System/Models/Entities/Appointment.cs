public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan Duration { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Reason { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    public virtual Invoice Invoice { get; set; }
}