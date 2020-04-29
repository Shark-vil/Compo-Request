using Compo_Shared_Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Database
{
    public class DatabaseContext : DbContext
    {
        private static string DbHost;
        private static string DbUser;
        private static string DbPassword;
        private static string DbDatabase;

        public DbSet<User> Users { get; set; }

        public static void Setup(string DbHost, string DbUser, string DbPassword, string DbDatabase)
        {
            DatabaseContext.DbHost = DbHost;
            DatabaseContext.DbUser = DbUser;
            DatabaseContext.DbPassword = DbPassword;
            DatabaseContext.DbDatabase = DbDatabase;
        }

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
            DbBuilder.UseMySql($"server={DbHost};UserId={DbUser};Password={DbPassword};database={DbDatabase};");
        }
    }
}
