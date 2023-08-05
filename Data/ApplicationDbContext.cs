using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewProject.Models;

namespace NewProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<TopicStart> Threads { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>().HasKey(u => u.Id);
            modelBuilder.Entity<TopicStart>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Answer>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
        }
    }
}