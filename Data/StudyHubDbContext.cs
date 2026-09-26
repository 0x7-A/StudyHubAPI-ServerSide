using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using System.Diagnostics.Metrics;

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
        public virtual DbSet<LoginInfo> LoginInfos { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Damage> Damages { get; set; }

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

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.PhoneNumber).IsUnique();

            });

            // =========================================================
            // 2. ADMINISTRATORS (TPT Child of Person)
            // =========================================================
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.ToTable("Administrators");
                entity.HasBaseType<Person>();

                entity.Property(e => e.HireDate)
                    .HasColumnType("DATE")
                    .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

                entity.Property(a => a.CountryID)
                      .IsRequired();

                entity.HasOne(a => a.Country)
                      .WithMany(c => c.Administrators)
                      .HasForeignKey(a => a.CountryID)
                      .HasConstraintName("FK_Administrators_Countries")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================================================
            // 3. CUSTOMERS (TPT Child of Person)
            // =========================================================
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.ToTable("Customers");
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

                entity.HasOne(r => r.Admin)
                    .WithMany(a => a.Reservation)
                    .HasForeignKey(r => r.AdminID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reservations_Administrators");

                entity.HasOne(r => r.Customer)
                    .WithMany(c => c.Reservation)
                    .HasForeignKey(r => r.CustomerID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reservations_Customers");

                entity.HasOne(r => r.Workspace)
                    .WithMany(w => w.Reservations)
                    .HasForeignKey(r => r.WorkspaceID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reservations_Workspaces");

                entity.HasIndex(r => r.CustomerID).HasDatabaseName("IX_Reservations_CustomerID");
                entity.HasIndex(r => r.WorkspaceID).HasDatabaseName("IX_Reservations_WorkspaceID");

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

                entity.HasOne(rev => rev.Reservation)
                    .WithMany(r => r.Reviews)
                    .HasForeignKey(rev => rev.ReservationID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Reviews_Reservations");

                entity.HasIndex(e => e.ReservationID).HasDatabaseName("IX_Reviews_ReservationID");

                entity.ToTable(t => t.HasCheckConstraint("CHK_Reviews_Rate", "[Rate] BETWEEN 1 AND 5"));
            });

            // =============================================
            // 7. WORKSPACE IMAGES
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

                entity.HasIndex(e => e.WorkspaceID, "IX_WorkspaceImages_WorkspaceID");

                entity.HasOne(e => e.Workspace)
                      .WithMany(w => w.WorkspaceImages)
                      .HasForeignKey(e => e.WorkspaceID)
                      .HasConstraintName("FK_WorkspaceImages_Workspaces_WorkspaceID")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =============================================
            // 8. PAYMENTS
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

                entity.HasIndex(e => e.CustomerID, "IX_Payments_CustomerID");
                entity.HasIndex(e => e.AdminID, "IX_Payments_AdminID");
                entity.HasIndex(e => e.ReservationID, "IX_Payments_ReservationID");

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

            // =============================================
            // 9. REFRESH TOKENS
            // =============================================
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");

                entity.HasKey(e => e.Id)
                      .HasName("PK_RefreshTokens");

                entity.Property(e => e.TokenHash)
                      .IsRequired()
                      .HasColumnType("nvarchar(max)");

                entity.Property(e => e.ExpiresAt)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(e => e.RevokedAt)
                      .HasColumnType("datetime2");

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Ignore(e => e.IsActive);

                entity.HasIndex(e => e.PersonID)
                      .HasDatabaseName("IX_RefreshTokens_PersonID");


                entity.HasOne(e => e.loginfo)
                      .WithMany()
                      .HasForeignKey(e => e.PersonID)
                      .HasConstraintName("FK_RefreshTokens_Person_PersonID")
                      .OnDelete(DeleteBehavior.Cascade);
            });



            // =============================================
            // 10. Login Info
            // =============================================

            modelBuilder.Entity<LoginInfo>(entity =>
            {
                entity.ToTable("LoginInfo", t =>
                {
                    // Check constraint allowing roles 1 through 4 (includes "None")
                    t.HasCheckConstraint("CHK_LoginInfo_Role", "[Role] BETWEEN 1 AND 4");
                });

                // Primary Key
                entity.HasKey(e => e.LoginID)
                      .HasName("PK_LoginInfo");

                // Column Properties & Data Types
                entity.Property(e => e.LoginID)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(254)
                      .HasColumnType("varchar(254)");

                entity.Property(e => e.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(255)
                      .HasColumnType("varchar(255)");

                entity.Property(e => e.Role)
                      .IsRequired()
                      .HasColumnType("tinyint");

                // Unique Indexes
                entity.HasIndex(e => e.PersonID)
                      .IsUnique()
                      .HasDatabaseName("UQ_LoginInfo_PersonID");

                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasDatabaseName("UQ_LoginInfo_Email");

                // 1-to-1 Relationship with Person (Cascade Delete)
                entity.HasOne(l => l.Person)
                      .WithOne(p => p.LoginInfo)
                      .HasForeignKey<LoginInfo>(l => l.PersonID)
                      .HasConstraintName("FK_LoginInfo_Person_PersonID")
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // =============================================
            // 11. Countries
            // =============================================

            modelBuilder.Entity<Countries>(entity =>
            {
                // Table mapping
                entity.ToTable("Countries");

                // Primary Key
                entity.HasKey(c => c.CountryID);

                // Identity column: IDENTITY(1,1)
                entity.Property(c => c.CountryID)
                      .ValueGeneratedOnAdd();

                // Column: CountryName varchar(20) NOT NULL
                entity.Property(c => c.CountryName)
                      .HasColumnName("CountryName")
                      .HasMaxLength(20)
                      .IsUnicode(false) // maps to varchar instead of nvarchar
                      .IsRequired();
            });

            // =============================================
            // 11. Damages
            // =============================================


            modelBuilder.Entity<Damage>(entity =>
            {
                entity.ToTable("Damages");

                entity.HasKey(d => d.DamageId).HasName("PK_Damages");

                entity.Property(d => d.DamageId).HasColumnName("DamageID");
                entity.Property(d => d.PaymentId).HasColumnName("PaymentID");

                entity.Property(d => d.Notes).HasMaxLength(500);
                entity.Property(d => d.ImagePath).HasMaxLength(255);

                entity.HasOne(d => d.Payment)
                      .WithOne(p => p.Damages)
                      .HasForeignKey<Damage>(d => d.PaymentId)
                      .HasConstraintName("FK_Damages_Payments")
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

        }
    }
}