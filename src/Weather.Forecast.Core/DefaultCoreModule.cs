using Autofac;
using Weather.Forecast.Core.Interfaces;
using Weather.Forecast.Core.Services;

namespace Weather.Forecast.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<CityService>()
      .As<ICityService>().InstancePerLifetimeScope();

    builder.RegisterType<WeatherForecastService>()
      .As<IWeatherForecastService>().InstancePerLifetimeScope();
  }
}
