public class Prescription
{
    public int Id { get; set; }
    public int MedicalRecordId { get; set; }
    public int DoctorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SpecialInstructions { get; set; }
    
    // Navigation
    public virtual MedicalRecord MedicalRecord { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual ICollection<PrescriptionItem> Items { get; set; }
}