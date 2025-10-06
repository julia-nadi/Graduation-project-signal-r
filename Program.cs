using Microsoft.EntityFrameworkCore;
using signalR.Hubs;
using signalR.Data;
using signalR.Models;

namespace signalR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<ChatSession>();

     

        // Register DbContext with connection string from appsettings
        builder.Services.AddDbContext<dbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        builder.Services.AddScoped<ChatSessionRepository>();
        builder.Services.AddScoped<ChatRepository>(); // Add this line if missing
        // Other service registrations

        // ✅ Enable CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
                c.RoutePrefix = string.Empty; // Set to empty to serve Swagger UI at the app's root
            });

        }

        app.UseHttpsRedirection();

        // ✅ Use CORS before authorization
        app.UseCors("AllowAll");

        app.UseAuthorization();

        app.MapControllers();

        app.UseRouting();
        app.MapHub<ChatHub>("/chatHub");

        app.Run();
    }
}


