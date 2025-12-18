[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    
    public AccountController()
    {
        _context = new ApplicationDbContext();
        _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
        _signInManager = new SignInManager<ApplicationUser>(_userManager, HttpContext.GetOwinContext().Authentication);
        _emailService = new EmailService();
    }
    
    // GET: /Account/Login
    public ActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    
    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
        if (!ModelState.IsValid)
            return View(model);
            
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        
        switch (result)
        {
            case SignInStatus.Success:
                // Redirect based on role
                if (user.Role == UserRole.Admin)
                    return RedirectToAction("Dashboard", "Admin");
                else if (user.Role == UserRole.Doctor)
                    return RedirectToAction("Dashboard", "Doctor");
                else
                    return RedirectToAction("Dashboard", "Patient");
                    
            case SignInStatus.LockedOut:
                return View("Lockout");
            case SignInStatus.RequiresVerification:
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            default:
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
        }
    }
    
    // GET: /Account/Register (Patient only)
    public ActionResult Register()
    {
        return View();
    }
    
    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Phone = model.Phone,
                Role = UserRole.Patient,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Create patient profile
                var patient = new Patient
                {
                    UserId = user.Id,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Address = model.Address,
                    BloodGroup = model.BloodGroup,
                    EmergencyContact = model.EmergencyContact
                };
                
                _context.Patients.Add(patient);
                _context.SaveChanges();
                
                await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToAction("Dashboard", "Patient");
            }
            
            AddErrors(result);
        }
        
        return View(model);
    }
    
    // GET: /Account/ForgotPassword
    public ActionResult ForgotPassword()
    {
        return View();
    }
    
    // POST: /Account/ForgotPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ForgotPasswordConfirmation");
            }
            
            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", 
                new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                
            _emailService.SendPasswordResetEmail(model.Email, callbackUrl);
            
            return View("ForgotPasswordConfirmation");
        }
        
        return View(model);
    }
    
    // GET: /Account/ResetPassword
    public ActionResult ResetPassword(string code)
    {
        return code == null ? View("Error") : View();
    }
    
    // POST: /Account/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
            
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return RedirectToAction("ResetPasswordConfirmation");
            
        var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        if (result.Succeeded)
            return RedirectToAction("ResetPasswordConfirmation");
            
        AddErrors(result);
        return View();
    }
    
    // POST: /Account/LogOff
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        return RedirectToAction("Index", "Home");
    }
    
    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }
    
    private IAuthenticationManager AuthenticationManager
    {
        get { return HttpContext.GetOwinContext().Authentication; }
    }
}