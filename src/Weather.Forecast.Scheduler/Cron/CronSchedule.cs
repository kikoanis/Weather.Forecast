using System.Diagnostics;
using System.Globalization;

namespace Weather.Forecast.Scheduler.Cron;

/// <summary>
/// Represents a schedule initialized from the cron expression.
/// </summary>
[Serializable]
public sealed class CronSchedule
{
  private static readonly char[] Separators = { ' ' };
  private readonly CronField _days;
  private readonly CronField _daysOfWeek;
  private readonly CronField _hours;
  private readonly CronField _minutes;
  private readonly CronField _months;

  private CronSchedule(string expression)
  {
    Debug.Assert(expression != null);

    var fields = expression.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

    if (fields.Length != 5)
    {
      throw new FormatException(
        $"'{expression}' is not a valid cron expression. It must contain exactly 5 components of a schedule " +
        "(in the sequence of minutes, hours, days, months, days of week).");
    }

    _minutes = CronField.Minutes(fields[0]);
    _hours = CronField.Hours(fields[1]);
    _days = CronField.Days(fields[2]);
    _months = CronField.Months(fields[3]);
    _daysOfWeek = CronField.DaysOfWeek(fields[4]);
  }

  private static Calendar Calendar
  {
    get { return CultureInfo.InvariantCulture.Calendar; }
  }

  public static CronSchedule Parse(string expression)
  {
    if (expression == null)
    {
      throw new ArgumentNullException(nameof(expression));
    }

    return new CronSchedule(expression);
  }

  public IEnumerable<DateTime> GetNextOccurrences(DateTime baseTime, DateTime endTime)
  {
    for (var occurrence = GetNextOccurrence(baseTime, endTime);
         occurrence < endTime;
         occurrence = GetNextOccurrence(occurrence, endTime))
    {
      yield return occurrence;
    }
  }

  public DateTime GetNextOccurrence(DateTime baseTime)
  {
    return GetNextOccurrence(baseTime, DateTime.MaxValue);
  }

  public DateTime GetNextOccurrence(DateTime baseTime, DateTime endTime)
  {
    while (true)
    {
      const int nil = -1;

      var baseYear = baseTime.Year;
      var baseMonth = baseTime.Month;
      var baseDay = baseTime.Day;
      var baseHour = baseTime.Hour;
      var baseMinute = baseTime.Minute;

      var endYear = endTime.Year;
      var endMonth = endTime.Month;
      var endDay = endTime.Day;

      var year = baseYear;
      var month = baseMonth;
      var day = baseDay;
      var hour = baseHour;
      var minute = baseMinute + 1;

      // Minute
      minute = _minutes.Next(minute);

      if (minute == nil)
      {
        minute = _minutes.GetFirst();
        hour++;
      }

      // Hour
      hour = _hours.Next(hour);

      if (hour == nil)
      {
        minute = _minutes.GetFirst();
        hour = _hours.GetFirst();
        day++;
      }
      else if (hour > baseHour)
      {
        minute = _minutes.GetFirst();
      }

      // Day
      day = _days.Next(day);

      while (true)
      {
        if (day == nil)
        {
          minute = _minutes.GetFirst();
          hour = _hours.GetFirst();
          day = _days.GetFirst();
          month++;
        }
        else if (day > baseDay)
        {
          minute = _minutes.GetFirst();
          hour = _hours.GetFirst();
        }

        // Month
        month = _months.Next(month);

        if (month == nil)
        {
          minute = _minutes.GetFirst();
          hour = _hours.GetFirst();
          day = _days.GetFirst();
          month = _months.GetFirst();
          year++;
        }
        else if (month > baseMonth)
        {
          minute = _minutes.GetFirst();
          hour = _hours.GetFirst();
          day = _days.GetFirst();
        }

        var dateChanged = day != baseDay || month != baseMonth || year != baseYear;

        if (day > 28 && dateChanged && day > Calendar.GetDaysInMonth(year, month))
        {
          if (year >= endYear && month >= endMonth && day >= endDay) return endTime;

          day = nil;
          // try parsing again
          continue;
        }

        break;
      }

      var nextTime = new DateTime(year, month, day, hour, minute, 0, 0, baseTime.Kind);

      if (nextTime >= endTime) return endTime;

      // Day of week
      if (_daysOfWeek.Contains((int)nextTime.DayOfWeek))
      {
        return nextTime;
      }

      baseTime = new DateTime(year, month, day, 23, 59, 0, 0, baseTime.Kind);
    }
  }

  public override string ToString()
  {
    var writer = new StringWriter(CultureInfo.InvariantCulture);

    _minutes.Format(writer, true);
    writer.Write(' ');
    _hours.Format(writer, true);
    writer.Write(' ');
    _days.Format(writer, true);
    writer.Write(' ');
    _months.Format(writer, true);
    writer.Write(' ');
    _daysOfWeek.Format(writer, true);

    return writer.ToString();
  }
}
