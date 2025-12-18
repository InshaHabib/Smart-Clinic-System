public interface IDoctorService
{
    IEnumerable<Doctor> GetAllDoctors();
    IEnumerable<Doctor> GetAvailableDoctors();
    Doctor GetDoctorById(int id);
    Doctor GetDoctorByUserId(string userId);
    bool CreateDoctor(Doctor doctor, string email, string password);
    bool UpdateDoctor(Doctor doctor);
    bool DeleteDoctor(int id);
    IEnumerable<DoctorAvailability> GetDoctorAvailability(int doctorId);
    bool UpdateDoctorAvailability(int doctorId, List<DoctorAvailability> availabilities);
}