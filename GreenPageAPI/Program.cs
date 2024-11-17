using GreenPageAPI.Models;
using Microsoft.EntityFrameworkCore;
using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using DotNetEnv;


Env.Load("enviroment.env");


var builder = WebApplication.CreateBuilder(args);

// Configurar los servicios, incluyendo el acceso a appsettings.json
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();



// Agregar SignalR al contenedor de servicios
builder.Services.AddSignalR();

// Obtener la variable de entorno DB_PASSWORD
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Comprobar si dbPassword es null o vacío y manejarlo
if (string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("La variable de entorno DB_PASSWORD no está configurada.");
}

// Configurar el contexto de la base de datos
builder.Services.AddDbContext<GreenPageContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")?.Replace("{DB_PASSWORD}", dbPassword))
);


// Agregar el servicio CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        //builder.AllowAnyOrigin()   // Permite todos los orígenes
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();  
    });
});





// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.UseCors();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//app.MapControllers();
// Mapea el Hub de SignalR
app.MapHub<TimeHub>("/timeHub");
app.MapHub<TimeHub>("/auctionHub");


app.MapControllers();

app.Run();
