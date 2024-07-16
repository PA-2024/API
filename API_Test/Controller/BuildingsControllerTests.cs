using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using API_GesSIgn.Controllers;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public class BuildingsControllerTests : IDisposable
{
    private MonDbContext _context;
    private BuildingsController _controller;

    public BuildingsControllerTests()
    {
        _context = TestDbContextFactory.Create();

        SeedDatabase();

        _controller = new BuildingsController(_context);

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
        var school = new School { School_Id = 1, School_Name = "School1" };
        _context.Schools.Add(school);
        _context.Buildings.AddRange(
            new Building { Bulding_Id = 1, Bulding_City = "Paris", Bulding_Name = "ERAD", Bulding_Adress = "rue de toto", Bulding_School = school },
            new Building { Bulding_Id = 2, Bulding_City = "Paris", Bulding_Name = "ERAD", Bulding_Adress = "rue de toto", Bulding_School = school }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllBuildings_ReturnsOkResult_WithListOfBuildings()
    {
        var result = await _controller.GetAllBuildings();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var buildings = Assert.IsType<List<Building>>(okResult.Value);
        Assert.Equal(2, buildings.Count);
    }

    [Fact]
    public async Task GetBuildingDetails_ReturnsNotFound_WhenBuildingDoesNotExist()
    {
        var result = await _controller.GetBuildingDetails(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetBuildingDetails_ReturnsOkResult_WithBuilding()
    {
        var result = await _controller.GetBuildingDetails(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var building = Assert.IsType<Building>(okResult.Value);
        Assert.Equal(1, building.Bulding_Id);
    }

    [Fact]
    public async Task CreateBuilding_ReturnsCreatedAtActionResult()
    {
        var request = new CreateBuildingRequest
        {
            Bulding_City = "NewCity",
            Bulding_Name = "NewBuilding",
            Bulding_Adress = "NewAddress"
        };

        var result = await _controller.CreateBuilding(request);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var building = Assert.IsType<Building>(createdAtActionResult.Value);
        Assert.Equal("NewCity", building.Bulding_City);
    }

    [Fact]
    public async Task UpdateBuilding_ReturnsNoContent_WhenBuildingIsUpdated()
    {
        var request = new UpdateBuildingRequest
        {
            Bulding_City = "UpdatedCity",
            Bulding_Name = "UpdatedBuilding",
            Bulding_Adress = "UpdatedAddress"
        };

        var result = await _controller.UpdateBuilding(1, request);

        Assert.IsType<NoContentResult>(result);
        var building = await _context.Buildings.FindAsync(1);
        Assert.Equal("UpdatedCity", building.Bulding_City);
    }

    [Fact]
    public async Task DeleteBuilding_ReturnsNoContent_WhenBuildingIsDeleted()
    {
        var result = await _controller.DeleteBuilding(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await _context.Buildings.FindAsync(1));
    }

    public void Dispose()
    {
        TestDbContextFactory.Destroy(_context);
    }
}
