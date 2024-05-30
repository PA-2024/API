﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_GesSIgn.Migrations
{
    [DbContext(typeof(MonDbContext))]
    partial class MonDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<int>("School_Id")
                        .HasColumnType("int");

                    b.HasKey("Bulding_Id");

                    b.HasIndex("School_Id");

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

                    b.Property<bool>("Presence_Is")
                        .HasColumnType("bit");

                    b.Property<int>("Presence_Student_Id")
                        .HasColumnType("int");

                    b.Property<int>("Presence_SubjectsHour_Id")
                        .HasColumnType("int");

                    b.HasKey("Presence_Id");

                    b.HasIndex("Presence_Student_Id");

                    b.HasIndex("Presence_SubjectsHour_Id");

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
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("School_allowSite")
                        .HasColumnType("bit");

                    b.Property<string>("School_token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("School_Id");

                    b.ToTable("Schools");
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

                    b.Property<int>("Sectors_School_Id")
                        .HasColumnType("int");

                    b.HasKey("Sectors_Id");

                    b.HasIndex("Sectors_School_Id");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Student", b =>
                {
                    b.Property<int>("Student_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Student_Id"));

                    b.Property<int>("Student_Sector_Id")
                        .HasColumnType("int");

                    b.Property<int>("Student_User_Id")
                        .HasColumnType("int");

                    b.HasKey("Student_Id");

                    b.HasIndex("Student_Sector_Id");

                    b.HasIndex("Student_User_Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Subjects", b =>
                {
                    b.Property<int>("Subjects_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Subjects_Id"));

                    b.Property<string>("Subjects_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Subjects_User_Id")
                        .HasColumnType("int");

                    b.HasKey("Subjects_Id");

                    b.HasIndex("Subjects_User_Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("API_GesSIgn.Models.SubjectsHour", b =>
                {
                    b.Property<int>("SubjectsHour_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubjectsHour_Id"));

                    b.Property<DateTime>("SubjectsHour_DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SubjectsHour_DateStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubjectsHour_Room")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SubjectsHour_Sector_Id")
                        .HasColumnType("int");

                    b.HasKey("SubjectsHour_Id");

                    b.HasIndex("SubjectsHour_Sector_Id");

                    b.ToTable("SubjectsHour");
                });

            modelBuilder.Entity("API_GesSIgn.Models.User", b =>
                {
                    b.Property<int>("User_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("User_Id"));

                    b.Property<int>("User_RoleRoles_Id")
                        .HasColumnType("int");

                    b.Property<int?>("User_School_Id")
                        .HasColumnType("int");

                    b.Property<string>("User_email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("User_firstname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User_lastname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User_num")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User_password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("User_Id");

                    b.HasIndex("User_RoleRoles_Id");

                    b.HasIndex("User_School_Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Building", b =>
                {
                    b.HasOne("API_GesSIgn.Models.School", "Bulding_School")
                        .WithMany()
                        .HasForeignKey("School_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bulding_School");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Presence", b =>
                {
                    b.HasOne("API_GesSIgn.Models.Student", "Presence_Student")
                        .WithMany()
                        .HasForeignKey("Presence_Student_Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("API_GesSIgn.Models.SubjectsHour", "Presence_SubjectsHour")
                        .WithMany()
                        .HasForeignKey("Presence_SubjectsHour_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Presence_Student");

                    b.Navigation("Presence_SubjectsHour");
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
                        .HasForeignKey("Sectors_School_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sectors_School");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Student", b =>
                {
                    b.HasOne("API_GesSIgn.Models.Sectors", "Student_Sectors")
                        .WithMany()
                        .HasForeignKey("Student_Sector_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_GesSIgn.Models.User", "Student_User")
                        .WithMany()
                        .HasForeignKey("Student_User_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student_Sectors");

                    b.Navigation("Student_User");
                });

            modelBuilder.Entity("API_GesSIgn.Models.Subjects", b =>
                {
                    b.HasOne("API_GesSIgn.Models.User", "Subjects_User")
                        .WithMany()
                        .HasForeignKey("Subjects_User_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subjects_User");
                });

            modelBuilder.Entity("API_GesSIgn.Models.SubjectsHour", b =>
                {
                    b.HasOne("API_GesSIgn.Models.Sectors", "SubjectsHour_Sectors")
                        .WithMany()
                        .HasForeignKey("SubjectsHour_Sector_Id")
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

                    b.HasOne("API_GesSIgn.Models.School", "User_School")
                        .WithMany()
                        .HasForeignKey("User_School_Id");

                    b.Navigation("User_Role");

                    b.Navigation("User_School");
                });
#pragma warning restore 612, 618
        }
    }
}
