using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using RepositoryLayer.Service;

namespace RepositoryLayer.Context
{
    public class GreetingDBContext:DbContext
    {
        public GreetingDBContext(DbContextOptions<GreetingDBContext> options) : base(options) { }

        public DbSet<GreetingEntity> Greetings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreetingEntity>()
                .HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, greetings are also deleted
        }
    }
}
