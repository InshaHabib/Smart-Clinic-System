public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    
    public InvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Invoice> GetAllInvoices()
    {
        return _context.Invoices
            .Include(i => i.Patient.User)
            .Include(i => i.Appointment)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();
    }
    
    public IEnumerable<Invoice> GetPatientInvoices(int patientId)
    {
        return _context.Invoices
            .Where(i => i.PatientId == patientId)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();
    }
    
    public Invoice GetInvoiceById(int id)
    {
        return _context.Invoices
            .Include(i => i.Patient.User)
            .Include(i => i.Appointment.Doctor.User)
            .FirstOrDefault(i => i.Id == id);
    }
    
    public bool CreateInvoice(Invoice invoice)
    {
        try
        {
            invoice.InvoiceNumber = GenerateInvoiceNumber();
            invoice.CreatedAt = DateTime.Now;
            invoice.Status = InvoiceStatus.Unpaid;
            
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public bool UpdatePayment(int id, decimal paidAmount)
    {
        try
        {
            var invoice = _context.Invoices.Find(id);
            if (invoice == null) return false;
            
            invoice.PaidAmount += paidAmount;
            invoice.PaidAt = DateTime.Now;
            
            if (invoice.PaidAmount >= invoice.TotalAmount)
                invoice.Status = InvoiceStatus.Paid;
            else
                invoice.Status = InvoiceStatus.PartiallyPaid;
                
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private string GenerateInvoiceNumber()
    {
        var lastInvoice = _context.Invoices.OrderByDescending(i => i.Id).FirstOrDefault();
        int nextNumber = (lastInvoice?.Id ?? 0) + 1;
        return $"INV-{DateTime.Now:yyyyMM}-{nextNumber:D4}";
    }
    
    public byte[] GenerateInvoicePDF(int invoiceId)
    {
        var invoice = GetInvoiceById(invoiceId);
        if (invoice == null) return null;
        
        using (MemoryStream ms = new MemoryStream())
        {
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(document, ms);
            document.Open();
            
            // Header
            Font headerFont = FontFactory.GetFont("Arial", 20, Font.BOLD);
            Paragraph header = new Paragraph("INVOICE", headerFont);
            header.Alignment = Element.ALIGN_CENTER;
            document.Add(header);
            
            document.Add(new Paragraph("\n"));
            
            // Clinic Info
            Font boldFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font normalFont = FontFactory.GetFont("Arial", 11);
            
            document.Add(new Paragraph("Smart Clinic", boldFont));
            document.Add(new Paragraph("123 Medical Street, City", normalFont));
            document.Add(new Paragraph("Phone: +92-300-1234567", normalFont));
            
            document.Add(new Paragraph("\n"));
            
            // Invoice Details
            PdfPTable detailsTable = new PdfPTable(2);
            detailsTable.WidthPercentage = 100;
            detailsTable.SetWidths(new float[] { 1, 1 });
            
            detailsTable.AddCell(new Phrase("Invoice Number:", boldFont));
            detailsTable.AddCell(invoice.InvoiceNumber);
            detailsTable.AddCell(new Phrase("Date:", boldFont));
            detailsTable.AddCell(invoice.CreatedAt.ToString("dd/MM/yyyy"));
            detailsTable.AddCell(new Phrase("Patient Name:", boldFont));
            detailsTable.AddCell(invoice.Patient.User.FullName);
            detailsTable.AddCell(new Phrase("Doctor:", boldFont));
            detailsTable.AddCell("Dr. " + invoice.Appointment.Doctor.User.FullName);
            
            document.Add(detailsTable);
            
            document.Add(new Paragraph("\n"));
            
            // Items
            PdfPTable itemsTable = new PdfPTable(3);
            itemsTable.WidthPercentage = 100;
            itemsTable.SetWidths(new float[] { 3, 1, 1 });
            
            itemsTable.AddCell(new Phrase("Description", boldFont));
            itemsTable.AddCell(new Phrase("Qty", boldFont));
            itemsTable.AddCell(new Phrase("Amount", boldFont));
            
            itemsTable.AddCell("Consultation Fee");
            itemsTable.AddCell("1");
            itemsTable.AddCell($"Rs. {invoice.ConsultationFee:N2}");
            
            if (invoice.MedicineCost > 0)
            {
                itemsTable.AddCell("Medicines");
                itemsTable.AddCell("1");
                itemsTable.AddCell($"Rs. {invoice.MedicineCost:N2}");
            }
            
            document.Add(itemsTable);
            
            document.Add(new Paragraph("\n"));
            
            // Totals
            PdfPTable totalsTable = new PdfPTable(2);
            totalsTable.WidthPercentage = 50;
            totalsTable.HorizontalAlignment = Element.ALIGN_RIGHT;
            
            totalsTable.AddCell(new Phrase("Total Amount:", boldFont));
            totalsTable.AddCell($"Rs. {invoice.TotalAmount:N2}");
            totalsTable.AddCell(new Phrase("Paid Amount:", boldFont));
            totalsTable.AddCell($"Rs. {invoice.PaidAmount:N2}");
            totalsTable.AddCell(new Phrase("Balance:", boldFont));
            totalsTable.AddCell($"Rs. {(invoice.TotalAmount - invoice.PaidAmount):N2}");
            
            document.Add(totalsTable);
            
            document.Add(new Paragraph("\n\n"));
            document.Add(new Paragraph("Thank you for choosing Smart Clinic!", normalFont));
            
            document.Close();
            return ms.ToArray();
        }
    }
}