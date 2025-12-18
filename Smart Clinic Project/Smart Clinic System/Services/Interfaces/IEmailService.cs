public interface IEmailService
{
    void SendAppointmentConfirmation(string email, string doctorName, DateTime appointmentDate);
    void SendAppointmentStatusUpdate(string email, string status);
    void SendPasswordResetEmail(string email, string resetLink);
}