public interface IMedicalRecordService
{
    IEnumerable<MedicalRecord> GetPatientRecords(int patientId);
    MedicalRecord GetRecordById(int id);
    bool CreateRecord(MedicalRecord record);
    bool UpdateRecord(MedicalRecord record);
}