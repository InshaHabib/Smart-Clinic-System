public interface IAppointmentService
{
    IEnumerable<Appointment> GetAllAppointments();
    IEnumerable<Appointment> GetAppointmentsByPatient(int patientId);
    IEnumerable<Appointment> GetAppointmentsByDoctor(int doctorId);
    IEnumerable<Appointment> GetTodayAppointments();
    IEnumerable<Appointment> GetPendingAppointments();
    Appointment GetAppointmentById(int id);
    bool BookAppointment(Appointment appointment);
    bool UpdateAppointmentStatus(int id, AppointmentStatus status);
    bool CancelAppointment(int id);
    bool IsSlotAvailable(int doctorId, DateTime dateTime);
}