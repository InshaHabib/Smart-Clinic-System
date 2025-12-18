[Authorize]
[AuthorizeRoles(UserRole.Patient)]
public class PatientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;
    private readonly IPrescriptionService _prescriptionService;
    private readonly IInvoiceService _invoiceService;
    private Patient _currentPatient;
    
    public PatientController()
    {
        _context = new ApplicationDbContext();
        _appointmentService = new AppointmentService(_context, new EmailService());
        _doctorService = new DoctorService(_context, null);
        _prescriptionService = new PrescriptionService(_context);
        _invoiceService = new InvoiceService(_context);
    }
    
    private Patient CurrentPatient
    {
        get
        {
            if (_currentPatient == null)
            {
                string userId = User.Identity.GetUserId();
                _currentPatient = _context.Patients.FirstOrDefault(p => p.UserId == userId);
            }
            return _currentPatient;
        }
    }
    
 
    public ActionResult Dashboard()
    {
        var model = new PatientDashboardViewModel
        {
            Patient = CurrentPatient,
            UpcomingAppointments = _appointmentService.GetAppointmentsByPatient(CurrentPatient.Id)
                .Where(a => a.AppointmentDate >= DateTime.Now && a.Status == AppointmentStatus.Approved)
                .Take(3)
                .ToList(),
            RecentPrescriptions = _prescriptionService.GetPatientPrescriptions(CurrentPatient.Id)
                .Take(3)
                .ToList(),
            PendingInvoices = _invoiceService.GetPatientInvoices(CurrentPatient.Id)
                .Where(i => i.Status != InvoiceStatus.Paid)
                .ToList()
        };
        
        return View(model);
    }
    
   
    public ActionResult Doctors(string specialty, string search)
    {
        var doctors = _doctorService.GetAvailableDoctors().AsQueryable();
        
        if (!string.IsNullOrEmpty(specialty))
            doctors = doctors.Where(d => d.Specialty.Contains(specialty));
            
        if (!string.IsNullOrEmpty(search))
            doctors = doctors.Where(d => d.User.FullName.Contains(search) || d.Specialty.Contains(search));
            
        return View(doctors.ToList());
    }
    
   
    public ActionResult BookAppointment(int doctorId)
    {
        var doctor = _doctorService.GetDoctorById(doctorId);
        if (doctor == null)
            return HttpNotFound();
            
        var model = new BookAppointmentViewModel
        {
            Doctor = doctor,
            DoctorAvailability = _doctorService.GetDoctorAvailability(doctorId).ToList()
        };
        
        return View(model);
    }
    
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult BookAppointment(BookAppointmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            var appointment = new Appointment
            {
                PatientId = CurrentPatient.Id,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                Reason = model.Reason,
                Notes = model.Notes
            };
            
            if (_appointmentService.BookAppointment(appointment))
            {
                TempData["Success"] = "Appointment booked successfully! Waiting for approval.";
                return RedirectToAction("Appointments");
            }
            
            ModelState.AddModelError("", "Failed to book appointment. Slot may not be available.");
        }
        
        model.Doctor = _doctorService.GetDoctorById(model.DoctorId);
        model.DoctorAvailability = _doctorService.GetDoctorAvailability(model.DoctorId).ToList();
        return View(model);
    }
    
  
    public ActionResult Appointments()
    {
        var appointments = _appointmentService.GetAppointmentsByPatient(CurrentPatient.Id);
        return View(appointments);
    }
   
    [HttpPost]
    public JsonResult CancelAppointment(int id)
    {
        var appointment = _context.Appointments.Find(id);
        if (appointment == null || appointment.PatientId != CurrentPatient.Id)
            return Json(new { success = false, message = "Appointment not found" });
            
        var success = _appointmentService.CancelAppointment(id);
        return Json(new { success = success });
    }
    
   
    public ActionResult Prescriptions()
    {
        var prescriptions = _prescriptionService.GetPatientPrescriptions(CurrentPatient.Id);
        return View(prescriptions);
    }
    
   
    public ActionResult PrescriptionDetails(int id)
    {
        var prescription = _prescriptionService.GetPrescriptionById(id);
        if (prescription == null || prescription.MedicalRecord.PatientId != CurrentPatient.Id)
            return HttpNotFound();
            
        return View(prescription);
    }
    
    
    public ActionResult DownloadPrescription(int id)
    {
        var prescription = _prescriptionService.GetPrescriptionById(id);
        if (prescription == null || prescription.MedicalRecord.PatientId != CurrentPatient.Id)
            return HttpNotFound();
            
        var pdf = _prescriptionService.GeneratePrescriptionPDF(id);
        return File(pdf, "application/pdf", $"Prescription_{id}.pdf");
    }
    
    
    public ActionResult Invoices()
    {
        var invoices = _invoiceService.GetPatientInvoices(CurrentPatient.Id);
        return View(invoices);
    }
    
    
    public ActionResult InvoiceDetails(int id)
    {
        var invoice = _invoiceService.GetInvoiceById(id);
        if (invoice == null || invoice.PatientId != CurrentPatient.Id)
            return HttpNotFound();
            
        return View(invoice);
    }
    
    
    public ActionResult DownloadInvoice(int id)
    {
        var invoice = _invoiceService.GetInvoiceById(id);
        if (invoice == null || invoice.PatientId != CurrentPatient.Id)
            return HttpNotFound();
            
        var pdf = _invoiceService.GenerateInvoicePDF(id);
        return File(pdf, "application/pdf", $"Invoice_{invoice.InvoiceNumber}.pdf");
    }
    
    
    public ActionResult Profile()
    {
        return View(CurrentPatient);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Profile(Patient model)
    {
        if (ModelState.IsValid)
        {
            var patient = CurrentPatient;
            patient.DateOfBirth = model.DateOfBirth;
            patient.Gender = model.Gender;
            patient.Address = model.Address;
            patient.BloodGroup = model.BloodGroup;
            patient.EmergencyContact = model.EmergencyContact;
            patient.Allergies = model.Allergies;
            
            _context.SaveChanges();
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
        
        return View(model);
    }
}