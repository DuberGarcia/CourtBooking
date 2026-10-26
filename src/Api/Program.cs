using Api.Errors;
using Application.Courts;
using Application.Reservations;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CourtBooking")
    ?? throw new InvalidOperationException("Falta la cadena de conexion 'CourtBooking'");

// Add services to the container.
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddScoped<CreateCourt>();
builder.Services.AddScoped<CreateReservation>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();                        // formato estándar de errores
builder.Services.AddExceptionHandler<AppExceptionHandler>(); // registra tu manejador

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
