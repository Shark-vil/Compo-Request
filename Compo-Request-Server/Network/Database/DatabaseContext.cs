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
        public DbSet<TeamUser> TeamUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TeamProject> TeamProjects { get; set; }
        public DbSet<WebRequestParamsItem> WebRequestParamsItems { get; set; }
        public DbSet<WebRequestItem> WebRequestItems { get; set; }
        public DbSet<WebRequestDir> WebRequestDirs { get; set; }

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

            MBuilder.Entity<TeamGroup>()
                .HasIndex(u => new { u.Uid })
                .IsUnique();

            MBuilder.Entity<Project>()
                .HasIndex(u => new { u.Uid })
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder DbBuilder)
        {
            DbBuilder.UseMySql($"server={DbHost};UserId={DbUser};Password={DbPassword};database={DbDatabase};");
        }
    }
}
