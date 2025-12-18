public class DoctorService : IDoctorService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public DoctorService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public IEnumerable<Doctor> GetAllDoctors()
    {
        return _context.Doctors
            .Include(d => d.User)
            .ToList();
    }
    
    public IEnumerable<Doctor> GetAvailableDoctors()
    {
        return _context.Doctors
            .Include(d => d.User)
            .Where(d => d.IsAvailable && d.User.IsActive)
            .ToList();
    }
    
    public Doctor GetDoctorById(int id)
    {
        return _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Availabilities)
            .FirstOrDefault(d => d.Id == id);
    }
    
    public Doctor GetDoctorByUserId(string userId)
    {
        return _context.Doctors
            .Include(d => d.User)
            .FirstOrDefault(d => d.UserId == userId);
    }
    
    public bool CreateDoctor(Doctor doctor, string email, string password)
    {
        try
        {
            // Create user account
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = doctor.User.FullName,
                Phone = doctor.User.Phone,
                Role = UserRole.Doctor,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            
            var result = _userManager.Create(user, password);
            if (!result.Succeeded) return false;
            
            // Create doctor profile
            doctor.UserId = user.Id;
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public bool UpdateDoctor(Doctor doctor)
    {
        try
        {
            _context.Entry(doctor).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public bool DeleteDoctor(int id)
    {
        try
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor == null) return false;
            
            var user = _context.Users.Find(doctor.UserId);
            if (user != null)
            {
                user.IsActive = false;
                _context.SaveChanges();
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public IEnumerable<DoctorAvailability> GetDoctorAvailability(int doctorId)
    {
        return _context.DoctorAvailabilities
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.DayOfWeek)
            .ToList();
    }
    
    public bool UpdateDoctorAvailability(int doctorId, List<DoctorAvailability> availabilities)
    {
        try
        {
            // Remove existing
            var existing = _context.DoctorAvailabilities.Where(a => a.DoctorId == doctorId);
            _context.DoctorAvailabilities.RemoveRange(existing);
            
            // Add new
            foreach (var availability in availabilities)
            {
                availability.DoctorId = doctorId;
                _context.DoctorAvailabilities.Add(availability);
            }
            
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}