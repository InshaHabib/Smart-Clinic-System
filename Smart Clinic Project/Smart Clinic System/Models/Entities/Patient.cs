public class Patient
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string BloodGroup { get; set; }
    public string EmergencyContact { get; set; }
    public string Allergies { get; set; }
    
    // Navigation
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
}