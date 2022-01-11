using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Infrastructure.Data.Config;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
  public void Configure(EntityTypeBuilder<City> builder)
  {
    builder.Property(p => p.Title)
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(p => p.WoeId)
      .IsRequired();

  }
}
