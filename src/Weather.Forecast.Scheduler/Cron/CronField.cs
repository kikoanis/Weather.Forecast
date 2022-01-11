using System.Collections;
using System.Globalization;

namespace Weather.Forecast.Scheduler.Cron;

/// <summary>
/// Represents a single cron field.
/// Most of the ideas in CROn code taken from https://github.com/kdcllc/CronScheduler.AspNetCore
/// </summary>
[Serializable]
public sealed class CronField
{
  private readonly BitArray _bits;
  private readonly CronFieldImplementation _implementation;
  private int _maxValue;
  private int _minValue;

  private CronField(CronFieldImplementation implementation, string expression)
  {
    _implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
    _bits = new BitArray(implementation.ValueCount);

    _bits.SetAll(false);
    _minValue = int.MaxValue;
    _maxValue = -1;

    _implementation.Parse(expression, Accumulate);
  }

  #region ICrontabField Members

  /// <summary>
  /// Gets the first value of the field or -1.
  /// </summary>
  public int GetFirst()
  {
    return _minValue < int.MaxValue ? _minValue : -1;
  }

  /// <summary>
  /// Gets the next value of the field that occurs after the given 
  /// start value or -1 if there is no next value available.
  /// </summary>
  public int Next(int start)
  {
    if (start < _minValue)
      return _minValue;

    var startIndex = ValueToIndex(start);
    var lastIndex = ValueToIndex(_maxValue);

    for (var i = startIndex; i <= lastIndex; i++)
    {
      if (_bits[i])
        return IndexToValue(i);
    }

    return -1;
  }

  /// <summary>
  /// Determines if the given value occurs in the field.
  /// </summary>
  public bool Contains(int value) => _bits[ValueToIndex(value)];

  #endregion

  /// <summary>
  /// Parses a cron field expression given its kind.
  /// </summary>
  public static CronField Parse(CronFieldKind kind, string expression) => new(CronFieldImplementation.FromKind(kind), expression);

  /// <summary>
  /// Parses a cron field expression representing minutes.
  /// </summary>
  public static CronField Minutes(string expression) => new(CronFieldImplementation.Minute, expression);

  /// <summary>
  /// Parses a cron field expression representing hours.
  /// </summary>
  public static CronField Hours(string expression) => new(CronFieldImplementation.Hour, expression);

  /// <summary>
  /// Parses a cron field expression representing days in any given month.
  /// </summary>
  public static CronField Days(string expression) => new(CronFieldImplementation.Day, expression);

  /// <summary>
  /// Parses a cron field expression representing months.
  /// </summary>
  public static CronField Months(string expression) => new(CronFieldImplementation.Month, expression);

  /// <summary>
  /// Parses a cron field expression representing days of a week.
  /// </summary>
  public static CronField DaysOfWeek(string expression) => new(CronFieldImplementation.DayOfWeek, expression);

  private int IndexToValue(int index) => index + _implementation.MinValue;

  private int ValueToIndex(int value) => value - _implementation.MinValue;

  /// <summary>
  /// Accumulates the given range (start to end) and interval of values
  /// into the current set of the field.
  /// </summary>
  /// <remarks>
  /// To set the entire range of values representable by the field,
  /// set <param name="start" /> and <param name="end" /> to -1 and
  /// <param name="interval" /> to 1.
  /// </remarks>
  private void Accumulate(int start, int end, int interval)
  {
    var minValue = _implementation.MinValue;
    var maxValue = _implementation.MaxValue;

    if (start == end)
    {
      if (start < 0)
      {
        // We're setting the entire range of values.
        if (interval <= 1)
        {
          _minValue = minValue;
          _maxValue = maxValue;
          _bits.SetAll(true);
          return;
        }

        start = minValue;
        end = maxValue;
      }
      else
      {
        // We're only setting a single value - check that it is in range.
        if (start < minValue)
        {
          throw new FormatException(
            $"'{start} is lower than the minimum allowable value for this field. Value must be between {_implementation.MinValue} and {_implementation.MaxValue} (all inclusive).");
        }

        if (start > maxValue)
        {
          throw new FormatException(
            $"'{end} is higher than the maximum allowable value for this field. Value must be between {_implementation.MinValue} and {_implementation.MaxValue} (all inclusive).");
        }
      }
    }
    else
    {
      // For ranges, if the start is bigger than the end value then
      // swap them over.
      if (start > end)
      {
        end ^= start;
        start ^= end;
        end ^= start;
      }

      if (start < 0)
      {
        start = minValue;
      }
      else if (start < minValue)
      {
        throw new FormatException(
          $"'{start} is lower than the minimum allowable value for this field. Value must be between {_implementation.MinValue} and {_implementation.MaxValue} (all inclusive).");
      }

      if (end < 0)
      {
        end = maxValue;
      }
      else if (end > maxValue)
      {
        throw new FormatException(
          $"'{end} is higher than the maximum allowable value for this field. Value must be between {_implementation.MinValue} and {_implementation.MaxValue} (all inclusive).");
      }
    }

    if (interval < 1)
    {
      interval = 1;
    }

    int i;

    // Populate the _bits table by setting all the bits corresponding to
    // the valid field values.
    for (i = start - minValue; i <= (end - minValue); i += interval)
      _bits[i] = true;

    // Make sure we remember the minimum value set so far Keep track of
    // the highest and lowest values that have been added to this field
    // so far.
    if (_minValue > start)
    {
      _minValue = start;
    }

    i += (minValue - interval);

    if (_maxValue < i)
    {
      _maxValue = i;
    }
  }

  public override string ToString()
  {
    return ToString(null);
  }

  public string ToString(string? format)
  {
    var writer = new StringWriter(CultureInfo.InvariantCulture);

    switch (format)
    {
      case "G":
      case null:
        Format(writer, true);
        break;
      case "N":
        Format(writer);
        break;
      default:
        throw new FormatException();
    }

    return writer.ToString();
  }

  public void Format(TextWriter writer)
  {
    Format(writer, false);
  }

  public void Format(TextWriter writer, bool noNames)
  {
    _implementation.Format(this, writer, noNames);
  }
}
