public interface IInvoiceService
{
    IEnumerable<Invoice> GetAllInvoices();
    IEnumerable<Invoice> GetPatientInvoices(int patientId);
    Invoice GetInvoiceById(int id);
    bool CreateInvoice(Invoice invoice);
    bool UpdatePayment(int id, decimal paidAmount);
    byte[] GenerateInvoicePDF(int invoiceId);
}