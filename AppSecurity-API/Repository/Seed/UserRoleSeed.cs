using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppSecurity_API.Repository.Seed
{
    public class UserRoleSeed : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                    new IdentityRole
                    {
                        Id = "6367f1be-3a1d-46d2-8dd2-c6ce7e9cac4f",
                        Name = "User",
                        NormalizedName = "USER",
                    },
                    new IdentityRole
                    {
                        Id = "a7f405b1-069b-43c4-9415-188625b318c0",
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    });
        }
    }
}
