public class EmailService : IEmailService
{
    private readonly string _smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
    private readonly int _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
    private readonly string _smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
    private readonly string _smtpPass = ConfigurationManager.AppSettings["SmtpPass"];
    
    public void SendAppointmentConfirmation(string email, string doctorName, DateTime appointmentDate)
    {
        string subject = "Appointment Confirmation - Smart Clinic";
        string body = $@"
            <h2>Appointment Confirmed</h2>
            <p>Your appointment has been booked successfully.</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Date & Time:</strong> {appointmentDate:dd MMM yyyy, hh:mm tt}</p>
            <p>Your appointment is pending approval. You will receive a confirmation email once approved.</p>
            <br/>
            <p>Thank you for choosing Smart Clinic!</p>
        ";
        
        SendEmail(email, subject, body);
    }
    
    public void SendAppointmentStatusUpdate(string email, string status)
    {
        string subject = $"Appointment {status} - Smart Clinic";
        string body = $@"
            <h2>Appointment Status Update</h2>
            <p>Your appointment status has been updated to: <strong>{status}</strong></p>
            <p>Please login to your account to view details.</p>
            <br/>
            <p>Thank you!</p>
        ";
        
        SendEmail(email, subject, body);
    }
    
    public void SendPasswordResetEmail(string email, string resetLink)
    {
        string subject = "Password Reset - Smart Clinic";
        string body = $@"
            <h2>Password Reset Request</h2>
            <p>You have requested to reset your password.</p>
            <p>Please click the link below to reset your password:</p>
            <a href='{resetLink}'>Reset Password</a>
            <p>This link will expire in 24 hours.</p>
            <p>If you did not request this, please ignore this email.</p>
        ";
        
        SendEmail(email, subject, body);
    }
    
    private void SendEmail(string to, string subject, string body)
    {
        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(_smtpUser);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                
                using (SmtpClient smtp = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"Email Error: {ex.Message}");
        }
    }
}