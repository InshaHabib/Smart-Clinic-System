using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartClinic.Models.Entities;

namespace SmartClinic.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships
            modelBuilder.Entity<Doctor>()
                .HasRequired(d => d.User)
                .WithOptional(u => u.Doctor)
                .WillCascadeOnDelete(false);
                
            modelBuilder.Entity<Patient>()
                .HasRequired(p => p.User)
                .WithOptional(u => u.Patient)
                .WillCascadeOnDelete(false);
                
            // Decimal precision
            modelBuilder.Entity<Doctor>()
                .Property(d => d.ConsultationFee)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Medicine>()
                .Property(m => m.UnitPrice)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Invoice>()
                .Property(i => i.PaidAmount)
                .HasPrecision(18, 2);
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}