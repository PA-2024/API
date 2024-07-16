using API_GesSIgn.Models;
using Microsoft.EntityFrameworkCore;
using System;

public class TestDbContextFactory
{
    public static MonDbContext Create()
    {
        var options = new DbContextOptionsBuilder<MonDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid().ToString())
            .Options;

        var context = new MonDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }

    public static void Destroy(MonDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
