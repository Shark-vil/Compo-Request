using Compo_Shared_Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Database
{
    public class DatabaseContext : DbContext
    {
        private static string DbHost = "localhost";
        private static string DbUser = "compo-request";
        private static string DbPassword = "HYOuv8pBtMdMgVlp";
        private static string DbDatabase = "compo-request";

        public DbSet<User> Users { get; set; }
        public DbSet<TeamGroup> TeamGroups { get; set; }

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
