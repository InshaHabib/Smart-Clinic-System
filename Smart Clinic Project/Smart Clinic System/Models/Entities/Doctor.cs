public class Doctor
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Specialty { get; set; }
    public string Bio { get; set; }
    public string PhotoPath { get; set; }
    public string Qualifications { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsAvailable { get; set; }
    
    // Navigation
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<DoctorAvailability> Availabilities { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
}