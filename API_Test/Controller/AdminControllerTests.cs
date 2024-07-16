using API_GesSIgn.Controllers;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace API_GesSIgn.Tests
{
    public class AdminControllerTests
    {
        private readonly MonDbContext _context;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _context = TestDbContextFactory.Create();;
            _controller = new AdminController(_context);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var school = new School { School_Id = 1, School_Name = "Test School" };
            var role = new Roles { Roles_Id = 1, Role_Name = "Gestion Ecole" };
            _context.Schools.Add(school);
            _context.Roles.Add(role);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAdminSchool_ShouldReturnOk()
        {
            var userRequest = new UserRequest
            {
                User_email = "test@test.com",
                User_num = "12345",
                User_firstname = "Test",
                User_lastname = "User",
                user_school_id = 1
            };

            var result = await _controller.CreateAdminSchool(userRequest);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("User Gestion Ecole ajouté", okResult.Value);
        }

    }
}
