
using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Data
{

    public partial class StudyHubDbContext : DbContext
    {

        public StudyHubDbContext(DbContextOptions<StudyHubDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }

        public virtual DbSet<Customers> Customers { get; set; }

        public virtual DbSet<Workspaces> Workspaces { get; set; }

        public virtual DbSet<Person> Person { get; set; }

        public virtual DbSet<Reservations> Reservations { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Payments> Payments { get; set; }

        public DbSet<WorkspaceImages> WorkspaceImages { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // 1. PERSON (Base Class for TPT Inheritance)
            // =========================================================
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");

                entity.HasKey(e => e.PersonID);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("VARCHAR(254)");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnType("tinyint");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnType("VARCHAR(255)");


                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);


                // Unique Constraints
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();

                // Check Constraint: CHK_Person_Role
                entity.ToTable(t => t.HasCheckConstraint("CHK_Person_Role", "[Role] BETWEEN 1 AND 3"));
            });

            // =========================================================
            // 2. ADMINISTRATORS (TPT Child of Person)
            // =========================================================
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.ToTable("Administrators");

                // PK is also the FK to Person(PersonID) in TPT
                entity.HasBaseType<Person>();

                entity.Property(e => e.HireDate)
                    .HasColumnType("DATE")
                    .HasDefaultValueSql("CAST(GETDATE() AS DATE)");
            });

            // =========================================================
            // 3. CUSTOMERS (TPT Child of Person)
            // =========================================================
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.ToTable("Customers");

                // PK is also the FK to Person(PersonID) in TPT
                entity.HasBaseType<Person>();

                entity.Property(e => e.RegisteredAt)
                    .HasColumnType("DATE")
                    .HasDefaultValueSql("CAST(GETDATE() AS DATE)");
            });

            // =========================================================
            // 4. WORKSPACES
            // =========================================================
            modelBuilder.Entity<Workspaces>(entity =>
            {
                entity.ToTable("Workspaces");

                entity.HasKey(e => e.WorkspaceID);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.WorkspaceStatus)
                    .IsRequired()
                    .HasColumnType("tinyint");

                entity.Property(e => e.HourlyRate)
                    .IsRequired()
                    .HasColumnType("DECIMAL(5,2)");

                entity.Property(e => e.MaximumCapacity)
                    .IsRequired();

                // Check Constraints
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Workspaces_status", "[WorkspaceStatus] BETWEEN 1 AND 2");
                    t.HasCheckConstraint("CHK_Workspaces_HourlyRate", "[HourlyRate] > 0");
                    t.HasCheckConstraint("CHK_Workspaces_MaximumCapacity", "[MaximumCapacity] > 0");
                });
            });

            // =========================================================
            // 5. RESERVATIONS
            // =========================================================
            modelBuilder.Entity<Reservations>(entity =>
            {
                entity.ToTable("Reservations");

                entity.HasKey(e => e.ReservationID);

                entity.Property(e => e.ReservationDate)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.StartDate)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.EndDate)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.ActualStartDate)
                    .HasColumnType("DATETIME");

                entity.Property(e => e.ActualEndDate)
                    .HasColumnType("DATETIME");

                entity.Property(e => e.ReservationStatus)
                    .IsRequired()
                    .HasColumnType("tinyint");

                // Foreign Key: AdminID -> Administrators(PersonID)
                entity.HasOne(r => r.Admin)
                    .WithMany(a => a.Reservation)
                    .HasForeignKey(r => r.AdminID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reservations_Administrators");

                // Foreign Key: CustomerID -> Customers(PersonID)
                entity.HasOne(r => r.Customer)
                    .WithMany(c => c.Reservation)
                    .HasForeignKey(r => r.CustomerID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reservations_Customers");

                // Foreign Key: WorkspaceID -> Workspaces(WorkspaceID)
                entity.HasOne(r => r.Workspace)
                    .WithMany(w => w.Reservations)
                    .HasForeignKey(r => r.WorkspaceID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reservations_Workspaces");

                // Indexes
                entity.HasIndex(r => r.CustomerID).HasDatabaseName("IX_Reservations_CustomerID");
                entity.HasIndex(r => r.WorkspaceID).HasDatabaseName("IX_Reservations_WorkspaceID");

                // Check Constraints
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CHK_Reservations_Status", "[ReservationStatus] BETWEEN 1 AND 6");
                    t.HasCheckConstraint("CHK_Reservations_DateRange", "[EndDate] > [StartDate]");
                    t.HasCheckConstraint("CHK_Reservations_ActualDates",
                        "[ActualStartDate] IS NULL OR [ActualEndDate] IS NULL OR [ActualEndDate] >= [ActualStartDate]");
                });
            });

            // =========================================================
            // 6. REVIEWS
            // =========================================================
            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.ToTable("Reviews");

                entity.HasKey(e => e.ReviewID);

                entity.Property(e => e.Rate)
                    .IsRequired()
                    .HasColumnType("tinyint");

                entity.Property(e => e.Comment)
                    .HasMaxLength(200);

                // Foreign Key: ReservationID -> Reservations(ReservationID) (1-to-1 or 1-to-0..1)
                entity.HasOne(rev => rev.Reservation)
                    .WithMany(r => r.Reviews) // Or .WithOne(res => res.Review) if you added navigation on Reservation
                    .HasForeignKey(rev => rev.ReservationID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Reviews_Reservations");

                // Index
                entity.HasIndex(e => e.ReservationID).HasDatabaseName("IX_Reviews_ReservationID");

                // Check Constraint
                entity.ToTable(t => t.HasCheckConstraint("CHK_Reviews_Rate", "[Rate] BETWEEN 1 AND 5"));
            });

            // =============================================
            // WorkspaceImage Configuration
            // =============================================
            modelBuilder.Entity<WorkspaceImages>(entity =>
            {
                entity.ToTable("WorkspaceImages", t =>
                {
                    t.HasCheckConstraint("CK_WorkspaceImages_ImagePath", "LEN(LTRIM(RTRIM([ImagePath]))) > 0");
                });

                entity.HasKey(e => e.ImageID)
                      .HasName("PK_WorkspaceImages_ImageID");

                entity.Property(e => e.ImagePath)
                      .IsRequired()
                      .HasMaxLength(500);

                // Index
                entity.HasIndex(e => e.WorkspaceID, "IX_WorkspaceImages_WorkspaceID");

                // Relationship
                entity.HasOne(e => e.Workspace)
                      .WithMany(w => w.WorkspaceImages)
                      .HasForeignKey(e => e.WorkspaceID)
                      .HasConstraintName("FK_WorkspaceImages_Workspaces_WorkspaceID")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =============================================
            // Payments Configuration
            // =============================================
            modelBuilder.Entity<Payments>(entity =>
            {
                entity.ToTable("Payments", t =>
                {
                    t.HasCheckConstraint("CK_Payments_TotalPrice", "[TotalPrice] >= 0");
                    t.HasCheckConstraint("CK_Payments_PaymentStatus", "[PaymentStatus] IN (1, 2, 3)");
                    t.HasCheckConstraint("CK_Payments_PaymentReason", "[PaymentReason] IN (1, 2, 3)");
                });

                entity.HasKey(e => e.PaymentID)
                      .HasName("PK_Payments_PaymentID");

                entity.Property(e => e.TotalPrice)
                      .HasColumnType("decimal(10, 2)")
                      .IsRequired();

                // Map Enum properties to tinyint
                entity.Property(e => e.PaymentStatus)
                      .HasColumnType("tinyint")
                      .HasDefaultValue(PaymentStatus.Pending)
                      .IsRequired();

                entity.Property(e => e.PaymentReason)
                      .HasColumnType("tinyint")
                      .HasDefaultValue(PaymentReason.Basic)
                      .IsRequired();

                entity.Property(e => e.PaymentMethod)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.PaymentDate)
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                // Indexes
                entity.HasIndex(e => e.CustomerID, "IX_Payments_CustomerID");
                entity.HasIndex(e => e.AdminID, "IX_Payments_AdminID");
                entity.HasIndex(e => e.ReservationID, "IX_Payments_ReservationID");

                // Relationships
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Payments)
                      .HasForeignKey(e => e.CustomerID)
                      .HasConstraintName("FK_Payments_Customers_CustomerID")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Administrator)
                      .WithMany(a => a.Payments)
                      .HasForeignKey(e => e.AdminID)
                      .HasConstraintName("FK_Payments_Administrators_AdminID")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Reservation)
                      .WithMany(r => r.Payments)
                      .HasForeignKey(e => e.ReservationID)
                      .HasConstraintName("FK_Payments_Reservations_ReservationID")
                      .OnDelete(DeleteBehavior.Cascade);
            });





        }

    }
}

