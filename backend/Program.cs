using backend.data;
using backend.InterFaces;
using backend.Repository;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(op =>
    op.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

var allowedOrigins = builder.Configuration.GetSection("allowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {

            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});


builder.Services.AddHttpClient<IFastApiService, FastApiService>();
builder.Services.AddScoped<IPdfRepository, PdfRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();

app.UseCors("ReactPolicy");

// app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapGet("/", () => "Server is running...");
app.MapControllers();

app.Run();
