using backend.data;
using backend.InterFaces;
using backend.Repository;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var allowedOrigins = builder.Configuration.GetSection("allowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    if (allowedOrigins != null && allowedOrigins.Length > 0)
    {
        options.AddPolicy("ReactPolicy", policy => policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
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
app.UseCors("ReactPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Server is running...");
app.MapControllers();
app.Run();

