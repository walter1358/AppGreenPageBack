using GreenPageAPI.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Agregar SignalR al contenedor de servicios
builder.Services.AddSignalR();

// Configurar servicios y base de datos
builder.Services.AddDbContext<GreenPageContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Configura Kestrel para que use el puerto 44373 en HTTPS
//builder.WebHost.ConfigureKestrel(options =>
//{
    //options.ListenLocalhost(44373, listenOptions =>
    //{
    //    listenOptions.UseHttps(); // Habilita HTTPS en este puerto
    //});
//});

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



app.Run();
