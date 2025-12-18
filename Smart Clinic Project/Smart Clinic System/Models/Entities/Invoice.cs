public class Invoice
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal MedicineCost { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    
    // Navigation
    public virtual Appointment Appointment { get; set; }
    public virtual Patient Patient { get; set; }
}