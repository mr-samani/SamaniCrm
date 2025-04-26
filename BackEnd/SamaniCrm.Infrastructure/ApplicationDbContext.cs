using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Infrastructure.Identity;
using RefreshToken = SamaniCrm.Domain.Entities.RefreshToken;

namespace SamaniCrm.Infrastructure
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");
                b.Property(e => e.FirstName).HasMaxLength(50);
                b.Property(e => e.LastName).HasMaxLength(50);
                b.Property(e => e.Address).HasMaxLength(200);
                b.Property(e => e.PhoneNumber).HasMaxLength(15);
                b.Property(e => e.ProfilePicture).HasMaxLength(200);
            });
        }
    }
}
