[Authorize]
[AuthorizeRoles(UserRole.Admin)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDoctorService _doctorService;
    private readonly IAppointmentService _appointmentService;
    private readonly IInvoiceService _invoiceService;
    
    public AdminController()
    {
        _context = new ApplicationDbContext();
        _doctorService = new DoctorService(_context, new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context)));
        _appointmentService = new AppointmentService(_context, new EmailService());
        _invoiceService = new InvoiceService(_context);
    }
    

    public ActionResult Dashboard()
    {
        var model = new AdminDashboardViewModel
        {
            TotalPatients = _context.Patients.Count(),
            TotalDoctors = _context.Doctors.Count(),
            TodayAppointments = _appointmentService.GetTodayAppointments().Count(),
            PendingAppointments = _appointmentService.GetPendingAppointments().Count(),
            TotalRevenue = _context.Invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => (decimal?)i.PaidAmount) ?? 0,
            RecentAppointments = _appointmentService.GetTodayAppointments().Take(5).ToList()
        };
        
        return View(model);
    }
    

    public ActionResult Doctors()
    {
        var doctors = _doctorService.GetAllDoctors();
        return View(doctors);
    }
    
    
    public ActionResult CreateDoctor()
    {
        return View();
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateDoctor(CreateDoctorViewModel model)
    {
        if (ModelState.IsValid)
        {
            var doctor = new Doctor
            {
                Specialty = model.Specialty,
                Bio = model.Bio,
                Qualifications = model.Qualifications,
                ConsultationFee = model.ConsultationFee,
                IsAvailable = true,
                User = new ApplicationUser
                {
                    FullName = model.FullName,
                    Phone = model.Phone
                }
            };
            
            // Handle photo upload
            if (model.Photo != null && model.Photo.ContentLength > 0)
            {
                string fileName = Path.GetFileName(model.Photo.FileName);
                string path = Path.Combine(Server.MapPath("~/Uploads/Doctors/"), fileName);
                model.Photo.SaveAs(path);
                doctor.PhotoPath = "/Uploads/Doctors/" + fileName;
            }
            
            if (_doctorService.CreateDoctor(doctor, model.Email, model.Password))
            {
                TempData["Success"] = "Doctor created successfully!";
                return RedirectToAction("Doctors");
            }
            
            ModelState.AddModelError("", "Failed to create doctor.");
        }
        
        return View(model);
    }
    
   
    public ActionResult EditDoctor(int id)
    {
        var doctor = _doctorService.GetDoctorById(id);
        if (doctor == null)
            return HttpNotFound();
            
        var model = new EditDoctorViewModel
        {
            Id = doctor.Id,
            FullName = doctor.User.FullName,
            Email = doctor.User.Email,
            Phone = doctor.User.Phone,
            Specialty = doctor.Specialty,
            Bio = doctor.Bio,
            Qualifications = doctor.Qualifications,
            ConsultationFee = doctor.ConsultationFee,
            IsAvailable = doctor.IsAvailable,
            PhotoPath = doctor.PhotoPath
        };
        
        return View(model);
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditDoctor(EditDoctorViewModel model)
    {
        if (ModelState.IsValid)
        {
            var doctor = _doctorService.GetDoctorById(model.Id);
            if (doctor == null)
                return HttpNotFound();
                
            doctor.Specialty = model.Specialty;
            doctor.Bio = model.Bio;
            doctor.Qualifications = model.Qualifications;
            doctor.ConsultationFee = model.ConsultationFee;
            doctor.IsAvailable = model.IsAvailable;
            doctor.User.FullName = model.FullName;
            doctor.User.Phone = model.Phone;
            
            // Handle photo upload
            if (model.Photo != null && model.Photo.ContentLength > 0)
            {
                string fileName = Path.GetFileName(model.Photo.FileName);
                string path = Path.Combine(Server.MapPath("~/Uploads/Doctors/"), fileName);
                model.Photo.SaveAs(path);
                doctor.PhotoPath = "/Uploads/Doctors/" + fileName;
            }
            
            if (_doctorService.UpdateDoctor(doctor))
            {
                TempData["Success"] = "Doctor updated successfully!";
                return RedirectToAction("Doctors");
            }
            
            ModelState.AddModelError("", "Failed to update doctor.");
        }
        
        return View(model);
    }
    
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteDoctor(int id)
    {
        if (_doctorService.DeleteDoctor(id))
        {
            TempData["Success"] = "Doctor deleted successfully!";
        }
        else
        {
            TempData["Error"] = "Failed to delete doctor.";
        }
        
        return RedirectToAction("Doctors");
    }
    
    
    public ActionResult Patients()
    {
        var patients = _context.Patients
            .Include(p => p.User)
            .ToList();
        return View(patients);
    }
    
   
    public ActionResult Appointments()
    {
        var appointments = _appointmentService.GetPendingAppointments();
        return View(appointments);
    }
    
    
    [HttpPost]
    public JsonResult ApproveAppointment(int id)
    {
        var success = _appointmentService.UpdateAppointmentStatus(id, AppointmentStatus.Approved);
        return Json(new { success = success });
    }
    
    
    [HttpPost]
    public JsonResult RejectAppointment(int id)
    {
        var success = _appointmentService.CancelAppointment(id);
        return Json(new { success = success });
    }
    
    
    public ActionResult Medicines()
    {
        var medicines = _context.Medicines
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .ToList();
        return View(medicines);
    }

    public ActionResult CreateMedicine()
    {
        return View();
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateMedicine(Medicine model)
    {
        if (ModelState.IsValid)
        {
            model.CreatedAt = DateTime.Now;
            model.IsActive = true;
            _context.Medicines.Add(model);
            _context.SaveChanges();
            
            TempData["Success"] = "Medicine added successfully!";
            return RedirectToAction("Medicines");
        }
        
        return View(model);
    }
    
  
    public ActionResult EditMedicine(int id)
    {
        var medicine = _context.Medicines.Find(id);
        if (medicine == null)
            return HttpNotFound();
            
        return View(medicine);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditMedicine(Medicine model)
    {
        if (ModelState.IsValid)
        {
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
            
            TempData["Success"] = "Medicine updated successfully!";
            return RedirectToAction("Medicines");
        }
        
        return View(model);
    }
    
    
    public ActionResult Invoices()
    {
        var invoices = _invoiceService.GetAllInvoices();
        return View(invoices);
    }
    
   
    public ActionResult Reports()
    {
        var model = new ReportsViewModel
        {
            DailyRevenue = _context.Invoices
                .Where(i => DbFunctions.TruncateTime(i.CreatedAt) == DateTime.Today && i.Status == InvoiceStatus.Paid)
                .Sum(i => (decimal?)i.PaidAmount) ?? 0,
                
            MonthlyRevenue = _context.Invoices
                .Where(i => i.CreatedAt.Month == DateTime.Now.Month && i.CreatedAt.Year == DateTime.Now.Year && i.Status == InvoiceStatus.Paid)
                .Sum(i => (decimal?)i.PaidAmount) ?? 0,
                
            TotalAppointments = _context.Appointments.Count(),
            CompletedAppointments = _context.Appointments.Count(a => a.Status == AppointmentStatus.Completed)
        };
        
        return View(model);
    }
}