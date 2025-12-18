public class MedicalRecordService : IMedicalRecordService
{
    private readonly ApplicationDbContext _context;
    
    public MedicalRecordService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<MedicalRecord> GetPatientRecords(int patientId)
    {
        return _context.MedicalRecords
            .Include(m => m.Doctor.User)
            .Include(m => m.Prescriptions)
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.VisitDate)
            .ToList();
    }
    
    public MedicalRecord GetRecordById(int id)
    {
        return _context.MedicalRecords
            .Include(m => m.Patient.User)
            .Include(m => m.Doctor.User)
            .Include(m => m.Prescriptions.Select(p => p.Items.Select(i => i.Medicine)))
            .FirstOrDefault(m => m.Id == id);
    }
    
    public bool CreateRecord(MedicalRecord record)
    {
        try
        {
            record.CreatedAt = DateTime.Now;
            _context.MedicalRecords.Add(record);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public bool UpdateRecord(MedicalRecord record)
    {
        try
        {
            _context.Entry(record).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}