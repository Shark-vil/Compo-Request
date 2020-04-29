using Compo_Shared_Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder MBuilder)
        {
            MBuilder.Entity<User>()
                .HasIndex(u => new { u.Email, u.Login })
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder DbBuilder)
        {
            DbBuilder.UseMySql("server=localhost;UserId=compo-request;Password=HYOuv8pBtMdMgVlp;database=compo-request;");
        }
    }
}
