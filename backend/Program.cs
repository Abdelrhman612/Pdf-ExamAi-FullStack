using backend.data;
using backend.InterFaces;
using backend.Repository;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("allowedOrigins").Get<string[]>();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options =>
{
    if (allowedOrigins != null && allowedOrigins.Length > 0)
    {
        options.AddPolicy("React", builder => builder.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader());
    }
});

builder.Services.AddHttpClient<IFastApiService, FastApiService>();
builder.Services.AddScoped<IPdfRepository, PdfRepository>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseCors("React");
app.MapGet("/", () => "Server is running...");
app.Run();

