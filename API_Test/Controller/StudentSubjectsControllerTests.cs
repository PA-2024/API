using API_GesSIgn.Controllers;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class StudentSubjectsControllerTests
{
    private MonDbContext _context;
    private readonly StudentSubjectsController _controller;

    public StudentSubjectsControllerTests()
    {
        _context = TestDbContextFactory.Create(); ;
        _controller = new StudentSubjectsController(_context);
        SeedDatabase(_context);
    }

    private void SeedDatabase(MonDbContext context)
    {
        var subject = new Subjects { Subjects_Id = 1, Subjects_Name = "Math", Subjects_School_Id = 1 };
        var subjectHour = new SubjectsHour { SubjectsHour_Id = 1, SubjectsHour_Subjects_Id = 1, SubjectsHour_DateStart = DateTime.Now.AddDays(1), SubjectsHour_DateEnd = DateTime.Now.AddDays(1).AddHours(1) };

        var school = new School { School_Id = 1, School_Name = "Test School" };
        var role = new Roles { Roles_Id = 1, Role_Name = "Gestion Ecole" };
        var user = new User { User_Id = 1, User_email = "test" , User_School_Id = 1, User_firstname = "toto", User_lastname = "ttt", User_password = "test", User_num = ""};
        var sector = new Sectors { Sectors_Id = 1, Sectors_Name = "Test Sector" };
        var student = new Student { Student_Id = 3, Student_User_Id = 1, Student_Sector_Id = 1 };
        var student2 = new Student { Student_Id = 4, Student_User_Id = 1, Student_Sector_Id = 1 };

        context.Schools.Add(school);
        context.SaveChanges();
        context.Roles.Add(role);
        context.SaveChanges();
        context.Subjects.Add(subject);
        context.SaveChanges();
        context.Users.Add(user);
        context.SaveChanges();
        context.SubjectsHour.Add(subjectHour);
        context.SaveChanges();
        context.Sectors.Add(sector);
        context.SaveChanges();
        context.Students.Add(student);
        context.Students.Add(student2);
        context.SaveChanges();
    }

    [Fact]
    public async Task AddStudentToSubject_CreatesPresence()
    {
        var request = new StudentSubjectRequest
        {
            Student_Id = 4,
            Subject_Id = 1
        };
        var result = await _controller.AddStudentToSubject(request);

        var okResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(201, okResult.StatusCode);

        var presences = _context.Presences.Where(p => p.Presence_Student_Id == 4 && p.Presence_SubjectsHour_Id == 1).ToList();
        Assert.Single(presences);
    }

    [Fact]
    public async Task AddStudentsToSubject_CreatesPresences()
    {
        // Arrange
        var request = new AddStudentsToSubjectRequest
        {
            StudentIds = new List<int> { 1 },
            Subject_Id = 1
        };

        // Act
        var result = await _controller.AddStudentsToSubject(request);

        // Assert
        var okResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(201, okResult.StatusCode);

        var presences = _context.Presences.Where(p => p.Presence_Student_Id == 1 && p.Presence_SubjectsHour_Id == 1).ToList();
        Assert.Single(presences);
    }

    [Fact]
    public async Task RemoveStudentFromSubject_RemovesPresence()
    {
        var request = new StudentSubjectRequest
        {
            Student_Id = 1,
            Subject_Id = 1
        };

        await _controller.AddStudentToSubject(request);

        var result = await _controller.RemoveStudentFromSubject(request);

        var noContentResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);

      
    }
}
