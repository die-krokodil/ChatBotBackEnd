using Microsoft.OpenApi.Models;
using LlamaAI.Core.Models;
using LlamaAI.Core.Services;
using LlamaAI.DishService.Models;
using LlamaAI.DishService.Services;
using System.Text.Json;
using System.IO;
using Microsoft.EntityFrameworkCore;
using LlamaAI.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LlamaAI API", Version = "v1" });
});

// Configure LLaMA
builder.Services.Configure<LlamaConfig>(builder.Configuration.GetSection("LlamaConfig"));
builder.Services.AddSingleton<ILlamaService, LlamaService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddDbContext<LlamaDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MariaDb"),
        new MySqlServerVersion(new Version(10, 5, 0))));
builder.Services.AddSingleton<MenuPlan>(sp =>
{
    var dishService = sp.GetRequiredService<IDishService>();

    // Load dishes from JSON file
    var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "dishes.json");
    var jsonString = File.ReadAllText(jsonFilePath);
    var menuData = JsonSerializer.Deserialize<DishMenuData>(jsonString);

    if (menuData?.Dishes == null || !menuData.Dishes.Any())
    {
        throw new InvalidOperationException("No dishes found in the JSON file.");
    }

    // Generate menu plan
    return dishService.GenerateTwoWeekMenuPlanAsync(menuData.Dishes).GetAwaiter().GetResult();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Initialize LLaMA model
try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var llamaService = scope.ServiceProvider.GetRequiredService<ILlamaService>();
        
        logger.LogInformation("Starting LLaMA model initialization...");
        await llamaService.LoadModelAsync();
        logger.LogInformation("LLaMA model initialized successfully");
    }
}
catch (Exception ex)
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to initialize LLaMA model. Application will exit.");
    }
    Environment.ExitCode = 1;
    return;
}

app.Run();
