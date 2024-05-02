using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajoutez services au conteneur.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:8080", "https://localhost:8080") 
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<MonDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("MYAPP_CONNECTION_STRING")));


var app = builder.Build();

// Configurer le pipeline de requêtes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080"; // Utilise 8080 si aucun port n'est défini
app.Run("http://*:" + port);
