public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    
    public AppointmentService(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    public IEnumerable<Appointment> GetAllAppointments()
    {
        return _context.Appointments
            .Include(a => a.Patient.User)
            .Include(a => a.Doctor.User)
            .OrderByDescending(a => a.AppointmentDate)
            .ToList();
    }
    
    public IEnumerable<Appointment> GetAppointmentsByPatient(int patientId)
    {
        return _context.Appointments
            .Include(a => a.Doctor.User)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToList();
    }
    
    public IEnumerable<Appointment> GetAppointmentsByDoctor(int doctorId)
    {
        return _context.Appointments
            .Include(a => a.Patient.User)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToList();
    }
    
    public IEnumerable<Appointment> GetTodayAppointments()
    {
        var today = DateTime.Today;
        return _context.Appointments
            .Include(a => a.Patient.User)
            .Include(a => a.Doctor.User)
            .Where(a => DbFunctions.TruncateTime(a.AppointmentDate) == today)
            .OrderBy(a => a.AppointmentDate)
            .ToList();
    }
    
    public IEnumerable<Appointment> GetPendingAppointments()
    {
        return _context.Appointments
            .Include(a => a.Patient.User)
            .Include(a => a.Doctor.User)
            .Where(a => a.Status == AppointmentStatus.Pending)
            .OrderBy(a => a.AppointmentDate)
            .ToList();
    }
    
    public Appointment GetAppointmentById(int id)
    {
        return _context.Appointments
            .Include(a => a.Patient.User)
            .Include(a => a.Doctor.User)
            .FirstOrDefault(a => a.Id == id);
    }
    
    public bool BookAppointment(Appointment appointment)
    {
        try
        {
            if (!IsSlotAvailable(appointment.DoctorId, appointment.AppointmentDate))
                return false;
                
            appointment.Status = AppointmentStatus.Pending;
            appointment.CreatedAt = DateTime.Now;
            appointment.Duration = TimeSpan.FromMinutes(30); // Default 30 min
            
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
            
            // Send email notification
            var patient = _context.Patients.Include(p => p.User).FirstOrDefault(p => p.Id == appointment.PatientId);
            var doctor = _context.Doctors.Include(d => d.User).FirstOrDefault(d => d.Id == appointment.DoctorId);
            
            _emailService.SendAppointmentConfirmation(patient.User.Email, doctor.User.FullName, appointment.AppointmentDate);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public bool UpdateAppointmentStatus(int id, AppointmentStatus status)
    {
        try
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return false;
            
            appointment.Status = status;
            _context.SaveChanges();
            
            // Send notification email
            var patient = _context.Patients.Include(p => p.User).FirstOrDefault(p => p.Id == appointment.PatientId);
            _emailService.SendAppointmentStatusUpdate(patient.User.Email, status.ToString());
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public bool CancelAppointment(int id)
    {
        return UpdateAppointmentStatus(id, AppointmentStatus.Cancelled);
    }
    
    public bool IsSlotAvailable(int doctorId, DateTime dateTime)
    {
        var dayOfWeek = dateTime.DayOfWeek;
        var time = dateTime.TimeOfDay;
        
        // Check doctor availability
        var availability = _context.DoctorAvailabilities
            .FirstOrDefault(a => a.DoctorId == doctorId 
                && a.DayOfWeek == dayOfWeek 
                && a.IsAvailable
                && a.StartTime <= time 
                && a.EndTime >= time);
                
        if (availability == null) return false;
        
        // Check if slot is already booked
        var existingAppointment = _context.Appointments
            .Any(a => a.DoctorId == doctorId 
                && a.AppointmentDate == dateTime
                && a.Status != AppointmentStatus.Cancelled);
                
        return !existingAppointment;
    }
}