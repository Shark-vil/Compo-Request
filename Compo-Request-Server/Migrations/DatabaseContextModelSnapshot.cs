﻿// <auto-generated />
using System;
using Compo_Request_Server.Network.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Compo_Request_Server.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Compo_Shared_Data.Models.TeamGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("TeamUid")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("TeamGroups");
                });

            modelBuilder.Entity("Compo_Shared_Data.Models.TeamUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("TeamGroupId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TeamGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("TeamUser");
                });

            modelBuilder.Entity("Compo_Shared_Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Patronymic")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("Email", "Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Compo_Shared_Data.Models.TeamUser", b =>
                {
                    b.HasOne("Compo_Shared_Data.Models.TeamGroup", "TeamGroups")
                        .WithMany("TeamUsers")
                        .HasForeignKey("TeamGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Compo_Shared_Data.Models.User", "Users")
                        .WithMany("TeamUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
