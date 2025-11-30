using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. VERİTABANI SERVİSİ (EKSİK OLAN KISIM BURASIYDI) ---
// appsettings.json dosyasındaki "DefaultConnection" ismini okur
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. HARİCİ API SERVİSİ (TMDb ve Google Books için) ---
builder.Services.AddHttpClient<HariciApiService>();

// --- 3. DİĞER STANDART AYARLAR ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. OTOMATİK VERİTABANI OLUŞTURMA (Magic Code) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated(); // Veritabanı yoksa oluşturur!
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı oluşturulurken bir hata çıktı.");
    }
}

// Swagger'ı her ortamda açalım ki rahat test et
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();