using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Weather.Forecast.Core;
using Weather.Forecast.Infrastructure;
using Weather.Forecast.Infrastructure.Data;
using Weather.Forecast.Web;
using Microsoft.OpenApi.Models;
using Weather.Forecast.Scheduler;
using Weather.Forecast.Scheduler.Scheduling;
using Weather.Forecast.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

string connectionString = builder.Configuration.GetConnectionString("SqliteConnection"); 

builder.Services.AddDbContext(connectionString);


var allowSpecificOrigins = "allow-specific-origins";

builder.Services.AddCors(options =>
{
  options.AddPolicy(name: allowSpecificOrigins,
    builder =>
    {
      builder.WithOrigins("http://localhost:3000"); // add any client/consumer url to this list. For now only the web app local url
    });
});

// Uncomment the next line if you want to add API versioning 
// builder.Services.AddApiVersioning();

builder.Services.AddControllers();
//builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

// Add scheduled tasks & scheduler
builder.Services.AddSingleton<IScheduledTask, FetchCityWeatherForecastTask>();
builder.Services.AddScheduler((sender, args) =>
{
  Console.Write(args.Exception.Message);
  args.SetObserved();
});

// Add Swagger 
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather Forecast API", Version = "v1" });
  c.EnableAnnotations();
  c.UseApiEndpoints();
});

builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  config.Path = "/listservices";
});


builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule(new DefaultCoreModule());
  containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

// Add loggers. Replace with any suitable logger.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
app.UseRouting();
app.UseCors(allowSpecificOrigins);

app.UseHttpsRedirection();
// app.UseAuthorization(); // we don't need it for now
app.UseCookiePolicy();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast API V1"));

app.UseEndpoints(endpoints =>
{
  endpoints.MapDefaultControllerRoute();
});

// Seed Database
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;

  try
  {
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    context.Database.EnsureCreated();
    SeedData.Initialize(services);
  }
  catch (Exception ex)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB.");
  }
}

app.Run();
