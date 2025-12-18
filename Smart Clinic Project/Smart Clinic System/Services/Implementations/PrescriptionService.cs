public class PrescriptionService : IPrescriptionService
{
    private readonly ApplicationDbContext _context;
    
    public PrescriptionService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public Prescription GetPrescriptionById(int id)
    {
        return _context.Prescriptions
            .Include(p => p.MedicalRecord.Patient.User)
            .Include(p => p.Doctor.User)
            .Include(p => p.Items.Select(i => i.Medicine))
            .FirstOrDefault(p => p.Id == id);
    }
    
    public IEnumerable<Prescription> GetPatientPrescriptions(int patientId)
    {
        return _context.Prescriptions
            .Include(p => p.Doctor.User)
            .Include(p => p.Items.Select(i => i.Medicine))
            .Where(p => p.MedicalRecord.PatientId == patientId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }
    
    public bool CreatePrescription(Prescription prescription, List<PrescriptionItem> items)
    {
        try
        {
            prescription.CreatedAt = DateTime.Now;
            _context.Prescriptions.Add(prescription);
            _context.SaveChanges();
            
            // Add items and update stock
            foreach (var item in items)
            {
                item.PrescriptionId = prescription.Id;
                _context.PrescriptionItems.Add(item);
                
                // Deduct from inventory
                var medicine = _context.Medicines.Find(item.MedicineId);
                if (medicine != null)
                {
                    medicine.QuantityInStock -= 1; 
                }
            }
            
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public byte[] GeneratePrescriptionPDF(int prescriptionId)
    {
        var prescription = GetPrescriptionById(prescriptionId);
        if (prescription == null) return null;
        
        using (MemoryStream ms = new MemoryStream())
        {
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            document.Open();
            
            // Header
            Font headerFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            Paragraph header = new Paragraph("Smart Clinic - Prescription", headerFont);
            header.Alignment = Element.ALIGN_CENTER;
            document.Add(header);
            
            document.Add(new Paragraph("\n"));
            
            // Patient Details
            Font normalFont = FontFactory.GetFont("Arial", 12);
            document.Add(new Paragraph($"Patient: {prescription.MedicalRecord.Patient.User.FullName}", normalFont));
            document.Add(new Paragraph($"Doctor: Dr. {prescription.Doctor.User.FullName}", normalFont));
            document.Add(new Paragraph($"Date: {prescription.CreatedAt:dd/MM/yyyy}", normalFont));
            document.Add(new Paragraph($"Diagnosis: {prescription.MedicalRecord.Diagnosis}", normalFont));
            
            document.Add(new Paragraph("\n"));
            
            // Medicines Table
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            
            // Headers
            table.AddCell("Medicine");
            table.AddCell("Dosage");
            table.AddCell("Frequency");
            table.AddCell("Duration");
            table.AddCell("Instructions");
            
            // Data
            foreach (var item in prescription.Items)
            {
                table.AddCell(item.Medicine.Name);
                table.AddCell(item.Dosage);
                table.AddCell(item.Frequency);
                table.AddCell($"{item.DurationDays} days");
                table.AddCell(item.Instructions ?? "-");
            }
            
            document.Add(table);
            
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph($"Special Instructions: {prescription.SpecialInstructions ?? "None"}", normalFont));
            
            document.Add(new Paragraph("\n\n"));
            document.Add(new Paragraph("_______________________", normalFont));
            document.Add(new Paragraph("Doctor's Signature", normalFont));
            
            document.Close();
            writer.Close();
            
            return ms.ToArray();
        }
    }
}