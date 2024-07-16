using API_GesSIgn.Controllers;
using API_GesSIgn.Helpers;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace API_GesSIgn.Test.Controller
{
    public class QCMControllerTests
    {
        private readonly MonDbContext _context;
        private readonly QCMController _controller;

        public QCMControllerTests()
        {
            _context = TestDbContextFactory.Create(); 
            _controller = new QCMController(_context);

            SeedDatabase();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim("SchoolId", "1"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        private void SeedDatabase()
        {
            var school = new School { School_Id = 1, School_Name = "Test School" };
            var subject = new Subjects { Subjects_Id = 1, Subjects_Name = "Test Subject", Subjects_School_Id = 1 };
            var subjectHour = new SubjectsHour { SubjectsHour_Id = 1, SubjectsHour_Subjects_Id = 1, SubjectsHour_DateStart = DateTime.Now, SubjectsHour_DateEnd = DateTime.Now.AddHours(1) };
            var qcm = new QCM { QCM_Id = 1, QCM_SubjectHour_id = 1, QCM_Title = "Test QCM" };

            _context.Schools.Add(school);
            _context.Subjects.Add(subject);
            _context.SubjectsHour.Add(subjectHour);
            _context.QCMs.Add(qcm);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetQcm_ShouldReturnPaginatedResult()
        {
            var result = await _controller.GetQcm(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var paginatedResult = Assert.IsType<PaginatedResult<QCMDto>>(okResult.Value);

            Assert.Single(paginatedResult.Items);
            Assert.Equal(1, paginatedResult.PageNumber);
            Assert.Equal(10, paginatedResult.PageSize);
            Assert.Equal(1, paginatedResult.TotalItems);
            Assert.Equal(1, paginatedResult.TotalPages);
        }

        [Fact]
        public async Task AddQcm_ShouldReturnOk()
        {
            var createQCM = new CreateQCMRequest
            {
                Title = "New QCM",
                SubjectHour_id = 1,
                Questions = new List<CreateQuestionRequest>
                {
                    new CreateQuestionRequest
                    {
                        Text = "Question 1",
                        Options = new List<CreateOptionRequest>
                        {
                            new CreateOptionRequest { Text = "Option 1", IsCorrect = true },
                            new CreateOptionRequest { Text = "Option 2", IsCorrect = false }
                        }
                    }
                }
            };

            var result = await _controller.AddQcm(createQCM);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("QCM ajouté", okResult.Value);
        }

        [Fact]
        public async Task AddQcm_SubjectHourFalse()
        {
            var createQCM = new CreateQCMRequest
            {
                Title = "New QCM",
                SubjectHour_id = 45,
                Questions = new List<CreateQuestionRequest>
                {
                    new CreateQuestionRequest
                    {
                        Text = "Question 1",
                        Options = new List<CreateOptionRequest>
                        {
                            new CreateOptionRequest { Text = "Option 1", IsCorrect = true },
                            new CreateOptionRequest { Text = "Option 2", IsCorrect = false }
                        }
                    }
                }
            };

            var result = await _controller.AddQcm(createQCM);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("SubjectHour not found", notFoundResult.Value);
        }
    }
}


