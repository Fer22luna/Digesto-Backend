using Backend.Data;
using Backend.Services;
using Backend.Services.Implementations;
using DotNetEnv; // from the DotNetEnv NuGet package

// load environment variables from a .env file (copy .env.example → .env and fill in real values)
// install the package with: `dotnet add package DotNetEnv`
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Supabase connection
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "https://ltpvyxrpmijkvrtyfpuh.supabase.co";
var supabaseKey = builder.Configuration["Supabase:AnonKey"] ?? "sb_publishable_BNuG6HJpkP2FuWTmXRwVhg_1AzywTG8";

builder.Services.AddScoped(sp => 
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    var logger = sp.GetRequiredService<ILogger<SupabaseConnection>>();
    return new SupabaseConnection(supabaseUrl, supabaseKey, httpClient, logger);
});

// Add services with dependency injection
builder.Services.AddScoped<IRegulationService, RegulationService>();
builder.Services.AddScoped<IUploadService, UploadService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must come before UseHttpsRedirection to avoid blocking preflight requests
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles(); 
app.MapControllers();

app.Run();
