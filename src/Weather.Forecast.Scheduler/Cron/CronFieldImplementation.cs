﻿using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace Weather.Forecast.Scheduler.Cron;

[Serializable]
public sealed class CronFieldImplementation : IObjectReference
{
  public static readonly CronFieldImplementation Minute = new(CronFieldKind.Minute, 0, 59, null);
  public static readonly CronFieldImplementation Hour = new(CronFieldKind.Hour, 0, 23, null);
  public static readonly CronFieldImplementation Day = new(CronFieldKind.Day, 1, 31, null);

  public static readonly CronFieldImplementation Month = new(CronFieldKind.Month, 1, 12,
    new[]
    {
      "January", "February", "March", "April",
      "May", "June", "July", "August",
      "September", "October", "November",
      "December"
    });

  public static readonly CronFieldImplementation DayOfWeek = new(CronFieldKind.DayOfWeek, 0, 6,
    new[]
    {
      "Sunday", "Monday", "Tuesday",
      "Wednesday", "Thursday", "Friday",
      "Saturday"
    });

  private static readonly CronFieldImplementation[] FieldByKind = { Minute, Hour, Day, Month, DayOfWeek };

  private static readonly CompareInfo Comparer = CultureInfo.InvariantCulture.CompareInfo;
  private static readonly char[] Comma = { ',' };

  private readonly string[]? _names;

  private CronFieldImplementation(CronFieldKind kind, int minValue, int maxValue, string[]? names)
  {
    Debug.Assert(Enum.IsDefined(typeof(CronFieldKind), kind));
    Debug.Assert(minValue >= 0);
    Debug.Assert(maxValue >= minValue);
    Debug.Assert(names == null || names.Length == (maxValue - minValue + 1));

    Kind = kind;
    MinValue = minValue;
    MaxValue = maxValue;
    _names = names;
  }

  public CronFieldKind Kind { get; }

  public int MinValue { get; }

  public int MaxValue { get; }

  public int ValueCount
  {
    get { return MaxValue - MinValue + 1; }
  }

  #region IObjectReference Members

  object IObjectReference.GetRealObject(StreamingContext context)
  {
    return FromKind(Kind);
  }

  #endregion

  public static CronFieldImplementation FromKind(CronFieldKind kind)
  {
    if (!Enum.IsDefined(typeof(CronFieldKind), kind))
    {
      throw new ArgumentException(
        $"Invalid cron field kind. Valid values are {string.Join(", ", Enum.GetNames(typeof(CronFieldKind)))}.", nameof(kind));
    }

    return FieldByKind[(int)kind];
  }
        
  public void Format(CronField field, TextWriter writer, bool noNames)
  {
    if (field == null)
      throw new ArgumentNullException(nameof(field));

    if (writer == null)
      throw new ArgumentNullException(nameof(writer));

    var next = field.GetFirst();
    var count = 0;

    while (next != -1)
    {
      var first = next;
      int last;

      do
      {
        last = next;
        next = field.Next(last + 1);
      } while (next - last == 1);

      if (count == 0
          && first == MinValue && last == MaxValue)
      {
        writer.Write('*');
        return;
      }

      if (count > 0)
        writer.Write(',');

      if (first == last)
      {
        FormatValue(first, writer, noNames);
      }
      else
      {
        FormatValue(first, writer, noNames);
        writer.Write('-');
        FormatValue(last, writer, noNames);
      }

      count++;
    }
  }

  private void FormatValue(int value, TextWriter writer, bool noNames)
  {
    Debug.Assert(writer != null);

    if (noNames || _names == null)
    {
      if (value is >= 0 and < 100)
      {
        FastFormatNumericValue(value, writer);
      }
      else
      {
        writer.Write(value.ToString(CultureInfo.InvariantCulture));
      }
    }
    else
    {
      var index = value - MinValue;
      writer.Write(_names[index]);
    }
  }

  private static void FastFormatNumericValue(int value, TextWriter writer)
  {
    Debug.Assert(value is >= 0 and < 100);
    Debug.Assert(writer != null);

    if (value >= 10)
    {
      writer.Write((char)('0' + (value / 10)));
      writer.Write((char)('0' + (value % 10)));
    }
    else
    {
      writer.Write((char)('0' + value));
    }
  }

  public void Parse(string str, CronFieldAccumulator acc)
  {
    if (acc == null)
      throw new ArgumentNullException(nameof(acc));

    if (string.IsNullOrEmpty(str))
      return;

    try
    {
      InternalParse(str, acc);
    }
    catch (FormatException e)
    {
      ThrowParseException(e, str);
    }
  }

  private static void ThrowParseException(Exception innerException, string str)
  {
    Debug.Assert(str != null);
    Debug.Assert(innerException != null);

    throw new FormatException($"'{str}' is not a valid cron field expression.",
      innerException);
  }

  private void InternalParse(string str, CronFieldAccumulator acc)
  {
    Debug.Assert(str != null);
    Debug.Assert(acc != null);

    if (str.Length == 0)
      throw new FormatException("A cron field value cannot be empty.");

    //
    // Next, look for a list of values (e.g. 1,2,3).
    //

    var commaIndex = str.IndexOf(",", StringComparison.Ordinal);

    if (commaIndex > 0)
    {
      foreach (var token in str.Split(Comma))
        InternalParse(token, acc);
    }
    else
    {
      var every = 1;

      //
      // Look for stepping first (e.g. */2 = every 2nd).
      // 

      var slashIndex = str.IndexOf("/", StringComparison.Ordinal);

      if (slashIndex > 0)
      {
        every = int.Parse(str.Substring(slashIndex + 1), CultureInfo.InvariantCulture);
        str = str.Substring(0, slashIndex);
      }

      //
      // Next, look for wildcard (*).
      //

      if (str.Length == 1 && str[0] == '*')
      {
        acc(-1, -1, every);
        return;
      }

      //
      // Next, look for a range of values (e.g. 2-10).
      //

      var dashIndex = str.IndexOf("-", StringComparison.Ordinal);

      if (dashIndex > 0)
      {
        var first = ParseValue(str.Substring(0, dashIndex));
        var last = ParseValue(str.Substring(dashIndex + 1));

        acc(first, last, every);
        return;
      }

      //
      // Finally, handle the case where there is only one number.
      //

      var value = ParseValue(str);

      if (every == 1)
      {
        acc(value, value, 1);
      }
      else
      {
        Debug.Assert(every != 0);

        acc(value, MaxValue, every);
      }
    }
  }

  private int ParseValue(string str)
  {
    Debug.Assert(str != null);

    if (str.Length == 0)
    {
      throw new FormatException("A cron field value cannot be empty.");
    }

    var firstChar = str[0];

    if (firstChar is >= '0' and <= '9')
    {
      return int.Parse(str, CultureInfo.InvariantCulture);
    }

    if (_names == null)
    {
      throw new FormatException(
        $"'{str}' is not a valid value for this cron field. It must be a numeric value between {MinValue} and {MaxValue} (all inclusive).");
    }

    for (var i = 0; i < _names.Length; i++)
    {
      if (Comparer.IsPrefix(_names[i], str, CompareOptions.IgnoreCase))
      {
        return i + MinValue;
      }
    }

    throw new FormatException(
      $"'{str}' is not a known value name. Use one of the following: {string.Join(", ", _names)}.");
  }
}
