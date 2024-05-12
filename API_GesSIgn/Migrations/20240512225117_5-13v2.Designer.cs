﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_GesSIgn.Migrations
{
    [DbContext(typeof(MonDbContext))]
    [Migration("20240512225117_5-13v2")]
    partial class _513v2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("API_GesSIgn.Models.Building", b =>
                {
                    b.Property<int>("Bulding_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Bulding_Id"));

                    b.Property<string>("Bulding_Adress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bulding_City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bulding_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Bulding_Id");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Error", b =>
                {
                    b.Property<int>("Error_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Error_id"));

                    b.Property<DateTime>("Error_DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Error_Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Error_Funtion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Error_Solved")
                        .HasColumnType("bit");

                    b.HasKey("Error_id");

                    b.ToTable("Errors");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Presence", b =>
                {
                    b.Property<int>("Presence_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Presence_Id"));

                    b.Property<Guid>("Prescence_Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Presence_SubjectsHourSubjectsHour_Id")
                        .HasColumnType("int");

                    b.Property<int>("Presence_UserUser_Id")
                        .HasColumnType("int");

                    b.HasKey("Presence_Id");

                    b.HasIndex("Presence_SubjectsHourSubjectsHour_Id");

                    b.HasIndex("Presence_UserUser_Id");

                    b.ToTable("Presences");
                });

            modelBuilder.Entity("API_GesSIgn.Models.QCM", b =>
                {
                    b.Property<int>("QCM_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QCM_Id"));

                    b.Property<bool>("QCM_Done")
                        .HasColumnType("bit");

                    b.Property<int>("QCM_TeacherUser_Id")
                        .HasColumnType("int");

                    b.HasKey("QCM_Id");

                    b.HasIndex("QCM_TeacherUser_Id");

                    b.ToTable("QCMs");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Roles", b =>
                {
                    b.Property<int>("Roles_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Roles_Id"));

                    b.Property<string>("Role_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Roles_Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("API_GesSIgn.Models.School", b =>
                {
                    b.Property<int>("School_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("School_Id"));

                    b.Property<DateTime>("School_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("School_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("School_allowSite")
                        .HasColumnType("bit");

                    b.Property<string>("School_token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("School_Id");

                    b.ToTable("School");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Sectors", b =>
                {
                    b.Property<int>("Sectors_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Sectors_Id"));

                    b.Property<string>("Sectors_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sectors_SchoolSchool_Id")
                        .HasColumnType("int");

                    b.HasKey("Sectors_Id");

                    b.HasIndex("Sectors_SchoolSchool_Id");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Student", b =>
                {
                    b.Property<int>("Student_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Student_Id"));

                    b.Property<string>("Student_FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Student_LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Student_UserUser_Id")
                        .HasColumnType("int");

                    b.Property<int>("Student_sectorsSectors_Id")
                        .HasColumnType("int");

                    b.HasKey("Student_Id");

                    b.HasIndex("Student_UserUser_Id");

                    b.HasIndex("Student_sectorsSectors_Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Subjects", b =>
                {
                    b.Property<int>("Subjects_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Subjects_Id"));

                    b.Property<int>("Subjects_SectorsSectors_Id")
                        .HasColumnType("int");

                    b.Property<int>("Subjects_UserUser_Id")
                        .HasColumnType("int");

                    b.HasKey("Subjects_Id");

                    b.HasIndex("Subjects_SectorsSectors_Id");

                    b.HasIndex("Subjects_UserUser_Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("API_GesSIgn.Models.SubjectsHour", b =>
                {
                    b.Property<int>("SubjectsHour_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubjectsHour_Id"));

                    b.Property<DateTime>("SubjectsHour_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubjectsHour_Rooom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SubjectsHour_SectorsSectors_Id")
                        .HasColumnType("int");

                    b.HasKey("SubjectsHour_Id");

                    b.HasIndex("SubjectsHour_SectorsSectors_Id");

                    b.ToTable("SubjectsHour");
                });

            modelBuilder.Entity("API_GesSIgn.Models.User", b =>
                {
                    b.Property<int>("User_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("User_Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("User_RoleRoles_Id")
                        .HasColumnType("int");

                    b.Property<string>("User_email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User_password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("User_Id");

                    b.HasIndex("User_RoleRoles_Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Presence", b =>
                {
                    b.HasOne("API_GesSIgn.Models.SubjectsHour", "Presence_SubjectsHour")
                        .WithMany()
                        .HasForeignKey("Presence_SubjectsHourSubjectsHour_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_GesSIgn.Models.User", "Presence_User")
                        .WithMany()
                        .HasForeignKey("Presence_UserUser_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Presence_SubjectsHour");

                    b.Navigation("Presence_User");
                });

            modelBuilder.Entity("API_GesSIgn.Models.QCM", b =>
                {
                    b.HasOne("API_GesSIgn.Models.User", "QCM_Teacher")
                        .WithMany()
                        .HasForeignKey("QCM_TeacherUser_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QCM_Teacher");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Sectors", b =>
                {
                    b.HasOne("API_GesSIgn.Models.School", "Sectors_School")
                        .WithMany()
                        .HasForeignKey("Sectors_SchoolSchool_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sectors_School");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Student", b =>
                {
                    b.HasOne("API_GesSIgn.Models.User", "Student_User")
                        .WithMany()
                        .HasForeignKey("Student_UserUser_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_GesSIgn.Models.Sectors", "Student_sectors")
                        .WithMany()
                        .HasForeignKey("Student_sectorsSectors_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student_User");

                    b.Navigation("Student_sectors");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Subjects", b =>
                {
                    b.HasOne("API_GesSIgn.Models.Sectors", "Subjects_Sectors")
                        .WithMany()
                        .HasForeignKey("Subjects_SectorsSectors_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_GesSIgn.Models.User", "Subjects_User")
                        .WithMany()
                        .HasForeignKey("Subjects_UserUser_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subjects_Sectors");

                    b.Navigation("Subjects_User");
                });

            modelBuilder.Entity("API_GesSIgn.Models.SubjectsHour", b =>
                {
                    b.HasOne("API_GesSIgn.Models.Sectors", "SubjectsHour_Sectors")
                        .WithMany()
                        .HasForeignKey("SubjectsHour_SectorsSectors_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubjectsHour_Sectors");
                });

            modelBuilder.Entity("API_GesSIgn.Models.User", b =>
                {
                    b.HasOne("API_GesSIgn.Models.Roles", "User_Role")
                        .WithMany()
                        .HasForeignKey("User_RoleRoles_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User_Role");
                });
#pragma warning restore 612, 618
        }
    }
}
