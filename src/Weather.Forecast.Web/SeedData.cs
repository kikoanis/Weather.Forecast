using Weather.Forecast.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Weather.Forecast.Web;

public static class SeedData
{
  public static void Initialize(IServiceProvider serviceProvider)
  {
    using var dbContext = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>(), null);

    if (dbContext.Cities.Any())
    {
      return;   // DB has been seeded
    }

    PopulateTestData(dbContext);
  }

  public static void PopulateTestData(AppDbContext dbContext)
  {
    dbContext.SaveChanges();
  }
}
