var builder = WebApplication.CreateBuilder(args);

// Ajoutez services au conteneur.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Utiliser le port de la variable d'environnement sur Heroku
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080"; // Utiliser 8080 si aucun port n'est défini
app.Run("http://*:" + port);
