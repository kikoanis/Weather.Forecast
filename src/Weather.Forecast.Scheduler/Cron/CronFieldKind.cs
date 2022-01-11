namespace Weather.Forecast.Scheduler.Cron;

[Serializable]
public enum CronFieldKind
{
  Minute,
  Hour,
  Day,
  Month,
  DayOfWeek
}
