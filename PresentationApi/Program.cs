using Application.Interfaces;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IAuditLogRepository>(provider => 
    new AuditLogRepository(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Server=localhost;Database=CallMiningDB;Trusted_Connection=true;TrustServerCertificate=true;"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
