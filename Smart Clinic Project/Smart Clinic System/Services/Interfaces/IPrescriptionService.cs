public interface IPrescriptionService
{
    Prescription GetPrescriptionById(int id);
    IEnumerable<Prescription> GetPatientPrescriptions(int patientId);
    bool CreatePrescription(Prescription prescription, List<PrescriptionItem> items);
    byte[] GeneratePrescriptionPDF(int prescriptionId);
}

