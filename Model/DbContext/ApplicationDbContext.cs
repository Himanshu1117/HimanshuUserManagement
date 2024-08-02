using Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> UpretiUsers { get; set; }
        public DbSet<Address> HimanshuAddress { get; set; }
        public DbSet<MasterAddress> MasterHimanshuAddresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();
                    sqlServerOptions.CommandTimeout(60);
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.User_Id)
                    .HasName("PK_User");

                entity.ToTable("UpretiUsers");

                entity.HasIndex(e => e.Email, "UQ_User_Email")
                    .IsUnique();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(20);

                entity.Property(e => e.LastName)
                    .HasMaxLength(20);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10);

                entity.Property(e => e.DateOfJoining)
                    .HasColumnType("date");

                entity.Property(e => e.DOB)
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(14);

                entity.Property(e => e.AlternatePhone)
                    .HasMaxLength(14);

                entity.Property(e => e.ImagePath)
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(e => e.Created_at)
                    .HasColumnType("datetime");

                entity.Property(e => e.Modified_at)
                    .HasColumnType("datetime");

                entity.Property(e => e.ResetPasswordToken)
                    .HasMaxLength(100);

                entity.Property(e => e.ResetExpiryToken)
                    .HasColumnType("datetime");

                // Relationships
                entity.HasMany(e => e.Addresses)
                      .WithOne(a => a.User)
                      .HasForeignKey(a => a.User_Id)
                      .HasConstraintName("FK_Address_User");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PK_Address");
                entity.HasKey(e => new { e.AId, e.User_Id }); // Composite key

                entity.ToTable("HimanshuAddress");

                entity.Property(e => e.City)
                    .HasMaxLength(30);

                entity.Property(e => e.State)
                    .HasMaxLength(30);

                entity.Property(e => e.Country)
                    .HasMaxLength(30);

                entity.Property(e => e.ZipCode)
                    .IsRequired();

                // Relationships
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Addresses)
                      .HasForeignKey(a => a.User_Id)
                      .HasConstraintName("FK_Address_User");

                entity.HasOne(a => a.MasterAddress)
                      .WithMany(m => m.Addresses)
                      .HasForeignKey(a => a.AId)
                      .HasConstraintName("FK_Address_MasterAddress");
            });

            modelBuilder.Entity<MasterAddress>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PK_MasterAddress");

                entity.ToTable("MasterHimanshuAddresses");

                entity.Property(e => e.AType)
                    .HasMaxLength(10);

                // Relationships
                entity.HasMany(e => e.Addresses)
                      .WithOne(a => a.MasterAddress)
                      .HasForeignKey(a => a.AId)
                      .HasConstraintName("FK_Address_MasterAddress");
            });
        }
    }
}


/*using Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> UpretiUsers { get; set; }
        public DbSet<Address> HimanshuAddress{ get; set; }
        public DbSet<MasterAddress> MasterHimanshuAddresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               var connectionString = _configuration.GetConnectionString("DefaultConnection");
               // var connectionString = "Server=172.24.0.101;Database=sDirect;User Id=sDirect;Password=sDirect;Trusted_Connection=True;Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=True;";
                optionsBuilder.UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();
                    sqlServerOptions.CommandTimeout(60);
                   
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.User_Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(20);
                entity.Property(e => e.MiddleName).HasMaxLength(20);
                entity.Property(e => e.LastName).HasMaxLength(20);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.DateOfJoining).HasColumnType("date");
                entity.Property(e => e.DOB).HasColumnType("date");
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.Phone).HasMaxLength(14);
                entity.Property(e => e.AlternatePhone).HasMaxLength(14);
                entity.Property(e => e.ImagePath).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(false);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.Created_at).HasColumnType("datetime");
                entity.Property(e => e.Modified_at).HasColumnType("datetime");
                entity.Property(e => e.ResetPasswordToken).HasMaxLength(100);
                entity.Property(e => e.ResetExpiryToken).HasColumnType("datetime");

                // Relationships
                entity.HasMany(e => e.Addresses)
                      .WithOne(a => a.User)
                      .HasForeignKey(a => a.User_Id);

            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.AId);
                entity.Property(e => e.City).HasMaxLength(30);
                entity.Property(e => e.State).HasMaxLength(30);
                entity.Property(e => e.Country).HasMaxLength(30);
                entity.Property(e => e.ZipCode).IsRequired();

                // Relationships
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Addresses)
                      .HasForeignKey(a => a.User_Id);
                entity.HasOne(a => a.MasterAddress)
                      .WithMany(m => m.Addresses)
                      .HasForeignKey(a => a.AId);
            });

            modelBuilder.Entity<MasterAddress>(entity =>
            {
                entity.HasKey(e => e.AId);
                entity.Property(e => e.AType).HasMaxLength(10);

                // Relationships
                entity.HasMany(e => e.Addresses)
                      .WithOne(a => a.MasterAddress)
                      .HasForeignKey(a => a.AId);
            });

          
        }
    }
}
*/