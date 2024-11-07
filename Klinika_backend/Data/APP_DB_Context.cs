using Microsoft.EntityFrameworkCore;
using Klinika_backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Klinika_backend.Data
{
    public class APP_DB_Context:DbContext
    {
        public APP_DB_Context(DbContextOptions<APP_DB_Context> options):base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var UserRoleId = "271a57cf-fc20-42ac-90de-54095c959f31";
            var DoctorRoleId = "eca0f843-7b9b-4cbe-8701-3db7ccae7411";
            var AdminRoleId = "b638bc65-51e5-4c4e-9519-78e34c6293d5";

            var roles = new List<IdentityRole>
    {
		new IdentityRole
        {
            Id = UserRoleId,
            ConcurrencyStamp = UserRoleId,
            Name="User",
            NormalizedName="User".ToUpper()
        },
        new IdentityRole
        {
            Id = DoctorRoleId,
            ConcurrencyStamp = DoctorRoleId,
            Name="Doctor",
            NormalizedName="Doctor".ToUpper()
        },
        new IdentityRole
        {
            Id = AdminRoleId,
            ConcurrencyStamp = AdminRoleId,
            Name="Admin",
            NormalizedName="Admin".ToUpper()
        }
    };
            builder.Entity<IdentityRole>().HasData(roles);
        }

        public DbSet<Klinika_backend.Models.Lekar> Lekar { get; set; } = default!;

    }
}
