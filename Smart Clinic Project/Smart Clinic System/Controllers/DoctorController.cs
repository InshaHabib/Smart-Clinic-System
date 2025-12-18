[Authorize]
[AuthorizeRoles(UserRole.Doctor)]
public class DoctorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAppointmentService _appointmentService;
    private readonly IMedicalRecordService _medicalRecordService;
    private readonly IPrescriptionService _prescriptionService;
    private readonly IDoctorService _doctorService;
    private Doctor _currentDoctor;
    
    public DoctorController()
    {
        _context = new ApplicationDbContext();
        _appointmentService = new AppointmentService(_context, new EmailService());
        _medicalRecordService = new MedicalRecordService(_context);
        _prescriptionService = new PrescriptionService(_context);
        _doctorService = new DoctorService(_context, null);
    }
    
    private Doctor CurrentDoctor
    {
        get
        {
            if (_currentDoctor == null)
            {
                string userId = User.Identity.GetUserId();
                _currentDoctor = _context.Doctors.FirstOrDefault(d => d.UserId == userId);
            }
            return _currentDoctor;
        }
    }
    
    
    public ActionResult Dashboard()
    {
        var model = new DoctorDashboardViewModel
        {
            Doctor = CurrentDoctor,
            TodayAppointments = _appointmentService.GetAppointmentsByDoctor(CurrentDoctor.Id)
                .Where(a => DbFunctions.TruncateTime(a.AppointmentDate) == DateTime.Today)
                .OrderBy(a => a.AppointmentDate)
                .ToList(),
            UpcomingAppointments = _appointmentService.GetAppointmentsByDoctor(CurrentDoctor.Id)
                .Where(a => a.AppointmentDate > DateTime.Now && a.Status == AppointmentStatus.Approved)
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToList(),
            TotalPatients = _context.MedicalRecords.Where(m => m.DoctorId == CurrentDoctor.Id).Select(m => m.PatientId).Distinct().Count()
        };
        
        return View(model);
    }
    
    
    public ActionResult Appointments(string status)
    {
        var appointments = _appointmentService.GetAppointmentsByDoctor(CurrentDoctor.Id);
        
        if (!string.IsNullOrEmpty(status))
        {
            AppointmentStatus statusEnum;
            if (Enum.TryParse(status, out statusEnum))
                appointments = appointments.Where(a => a.Status == statusEnum);
        }
        
        return View(appointments.OrderByDescending(a => a.AppointmentDate).ToList());
    }
    
 
    public ActionResult AppointmentDetails(int id)
    {
        var appointment = _appointmentService.GetAppointmentById(id);
        if (appointment == null || appointment.DoctorId != CurrentDoctor.Id)
            return HttpNotFound();
            
        var model = new AppointmentDetailsViewModel
        {
            Appointment = appointment,
            PatientRecords = _medicalRecordService.GetPatientRecords(appointment.PatientId).ToList()
        };
        
        return View(model);
    }
    
    [HttpPost]
    public JsonResult StartConsultation(int appointmentId)
    {
        var success = _appointmentService.UpdateAppointmentStatus(appointmentId, AppointmentStatus.Completed);
        return Json(new { success = success });
    }
    
    
    public ActionResult AddMedicalRecord(int appointmentId)
    {
        var appointment = _appointmentService.GetAppointmentById(appointmentId);
        if (appointment == null || appointment.DoctorId != CurrentDoctor.Id)
            return HttpNotFound();
            
        var model = new MedicalRecordViewModel
        {
            AppointmentId = appointmentId,
            PatientId = appointment.PatientId,
            DoctorId = CurrentDoctor.Id,
            VisitDate = DateTime.Now,
            Appointment = appointment
        };
        
        return View(model);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddMedicalRecord(MedicalRecordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var record = new MedicalRecord
            {
                AppointmentId = model.AppointmentId,
                PatientId = model.PatientId,
                DoctorId = CurrentDoctor.Id,
                VisitDate = model.VisitDate,
                ChiefComplaint = model.ChiefComplaint,
                Diagnosis = model.Diagnosis,
                Vitals = model.Vitals,
                Notes = model.Notes
            };
            
            if (_medicalRecordService.CreateRecord(record))
            {
                // Update appointment status
                _appointmentService.UpdateAppointmentStatus(model.AppointmentId, AppointmentStatus.Completed);
                
                TempData["Success"] = "Medical record added successfully!";
                TempData["RecordId"] = record.Id;
                return RedirectToAction("AddPrescription", new { medicalRecordId = record.Id });
            }
            
            ModelState.AddModelError("", "Failed to create medical record.");
        }
        
        model.Appointment = _appointmentService.GetAppointmentById(model.AppointmentId);
        return View(model);
    }
    
    
    public ActionResult AddPrescription(int medicalRecordId)
    {
        var record = _medicalRecordService.GetRecordById(medicalRecordId);
        if (record == null || record.DoctorId != CurrentDoctor.Id)
            return HttpNotFound();
            
        var model = new PrescriptionViewModel
        {
            MedicalRecordId = medicalRecordId,
            MedicalRecord = record,
            AvailableMedicines = _context.Medicines.Where(m => m.IsActive).OrderBy(m => m.Name).ToList(),
            Items = new List<PrescriptionItemViewModel>()
        };
        
        return View(model);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddPrescription(PrescriptionViewModel model)
    {
        if (ModelState.IsValid && model.Items != null && model.Items.Any())
        {
            var prescription = new Prescription
            {
                MedicalRecordId = model.MedicalRecordId,
                DoctorId = CurrentDoctor.Id,
                SpecialInstructions = model.SpecialInstructions
            };
            
            var items = model.Items.Select(i => new PrescriptionItem
            {
                MedicineId = i.MedicineId,
                Dosage = i.Dosage,
                Frequency = i.Frequency,
                DurationDays = i.DurationDays,
                Instructions = i.Instructions
            }).ToList();
            
            if (_prescriptionService.CreatePrescription(prescription, items))
            {
                TempData["Success"] = "Prescription created successfully!";
                return RedirectToAction("CreateInvoice", new { appointmentId = model.MedicalRecord.AppointmentId });
            }
            
            ModelState.AddModelError("", "Failed to create prescription.");
        }
        
        model.MedicalRecord = _medicalRecordService.GetRecordById(model.MedicalRecordId);
        model.AvailableMedicines = _context.Medicines.Where(m => m.IsActive).OrderBy(m => m.Name).ToList();
        return View(model);
    }
    
   
    public ActionResult CreateInvoice(int appointmentId)
    {
        var appointment = _appointmentService.GetAppointmentById(appointmentId);
        if (appointment == null || appointment.DoctorId != CurrentDoctor.Id)
            return HttpNotFound();
            
        var model = new InvoiceViewModel
        {
            AppointmentId = appointmentId,
            PatientId = appointment.PatientId,
            ConsultationFee = CurrentDoctor.ConsultationFee,
            Appointment = appointment
        };
        
        return View(model);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateInvoice(InvoiceViewModel model)
    {
        if (ModelState.IsValid)
        {
            var invoice = new Invoice
            {
                AppointmentId = model.AppointmentId,
                PatientId = model.PatientId,
                ConsultationFee = model.ConsultationFee,
                MedicineCost = model.MedicineCost,
                TotalAmount = model.ConsultationFee + model.MedicineCost,
                PaidAmount = 0
            };
            
            var invoiceService = new InvoiceService(_context);
            if (invoiceService.CreateInvoice(invoice))
            {
                TempData["Success"] = "Invoice created successfully!";
                return RedirectToAction("Appointments");
            }
            
            ModelState.AddModelError("", "Failed to create invoice.");
        }
        
        model.Appointment = _appointmentService.GetAppointmentById(model.AppointmentId);
        return View(model);
    }
    
   
    public ActionResult PatientRecords(int patientId)
    {
        var records = _medicalRecordService.GetPatientRecords(patientId);
        var patient = _context.Patients.Include(p => p.User).FirstOrDefault(p => p.Id == patientId);
        
        ViewBag.Patient = patient;
        return View(records);
    }
    
  
    public ActionResult Availability()
    {
        var availability = _doctorService.GetDoctorAvailability(CurrentDoctor.Id);
        return View(availability);
    }
    
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateAvailability(List<DoctorAvailability> availabilities)
    {
        if (_doctorService.UpdateDoctorAvailability(CurrentDoctor.Id, availabilities))
        {
            TempData["Success"] = "Availability updated successfully!";
        }
        else
        {
            TempData["Error"] = "Failed to update availability.";
        }
        
        return RedirectToAction("Availability");
    }
}