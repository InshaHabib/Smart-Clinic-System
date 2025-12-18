public class PrescriptionItem
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public int MedicineId { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public int DurationDays { get; set; }
    public string Instructions { get; set; }
    
    // Navigation
    public virtual Prescription Prescription { get; set; }
    public virtual Medicine Medicine { get; set; }
}