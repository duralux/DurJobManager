using System;
using System.Linq;
using System.Text;

namespace DurJobManager.Jobs
{
  [Flags]
  public enum DaysOfWeek
  {
    None = 0,
    Monday = 1,
    Tuesday = 1 << 2,
    Wednesday = 1 << 3,
    Thursday = 1 << 4,
    Friday = 1 << 5,
    Saturday = 1 << 6,
    Sunday = 1 << 7,
    All = Monday + Tuesday + Wednesday + Thursday + Friday + Saturday + Sunday
  }

  public static class DaysOfWeekExtension
  {

    #region Constants

    private static readonly char[] ALLOWED_DAYS = ['M', 'T', 'W', 'R', 'F', 'S', 'U'];

    #endregion


    #region Functions

    public static DaysOfWeek GetDaysOfWeek(this DayOfWeek dayOfWeek)
    {
      return dayOfWeek switch
      {
        DayOfWeek.Monday => DaysOfWeek.Monday,
        DayOfWeek.Tuesday => DaysOfWeek.Tuesday,
        DayOfWeek.Wednesday => DaysOfWeek.Wednesday,
        DayOfWeek.Thursday => DaysOfWeek.Thursday,
        DayOfWeek.Friday => DaysOfWeek.Friday,
        DayOfWeek.Saturday => DaysOfWeek.Saturday,
        DayOfWeek.Sunday => DaysOfWeek.Sunday,
        _ => throw new NotImplementedException(),
      };
    }


    public static bool HasDay(this DaysOfWeek daysOfWeek, DayOfWeek dayOfWeek)
    {
      return dayOfWeek switch
      {
        DayOfWeek.Monday => daysOfWeek.HasFlag(DaysOfWeek.Monday),
        DayOfWeek.Tuesday => daysOfWeek.HasFlag(DaysOfWeek.Tuesday),
        DayOfWeek.Wednesday => daysOfWeek.HasFlag(DaysOfWeek.Wednesday),
        DayOfWeek.Thursday => daysOfWeek.HasFlag(DaysOfWeek.Thursday),
        DayOfWeek.Friday => daysOfWeek.HasFlag(DaysOfWeek.Friday),
        DayOfWeek.Saturday => daysOfWeek.HasFlag(DaysOfWeek.Saturday),
        DayOfWeek.Sunday => daysOfWeek.HasFlag(DaysOfWeek.Sunday),
        _ => throw new ArgumentOutOfRangeException($"No proper weekday: '{dayOfWeek}'")
      };
    }


    public static string GetString(this DaysOfWeek daysOfWeek)
    {
      var w = new StringBuilder();
      if (daysOfWeek.HasFlag(DaysOfWeek.Monday))
      { w.Append('M'); }
      if (daysOfWeek.HasFlag(DaysOfWeek.Tuesday))
      { w.Append('T'); }
      if (daysOfWeek.HasFlag(DaysOfWeek.Wednesday))
      { w.Append('W'); }
      if (daysOfWeek.HasFlag(DaysOfWeek.Thursday))
      { w.Append('R'); }
      if (daysOfWeek.HasFlag(DaysOfWeek.Friday))
      { w.Append('F'); }
      if (daysOfWeek.HasFlag(DaysOfWeek.Saturday))
      { w.Append('S'); }
      if (daysOfWeek.HasFlag(DaysOfWeek.Sunday))
      { w.Append('U'); }
      return w.ToString();
    }


    public static DaysOfWeek GetDaysOfWeek(this string? text, bool emptyIsAll = true,
      bool throwException = true)
    {
      if (text == null && !emptyIsAll)
      { return DaysOfWeek.None; }

      if (text == null || String.IsNullOrWhiteSpace(text))
      { return DaysOfWeek.All; }

      if (throwException && text.Any(c => !ALLOWED_DAYS.Contains(c)))
      {
        throw new ArgumentException(
          $"Weekdays must be one of {String.Join(',', ALLOWED_DAYS)}");
      }

      DaysOfWeek daysOfWeek = DaysOfWeek.None;
      if (text.Contains('M'))
      { daysOfWeek |= DaysOfWeek.Monday; }
      if (text.Contains('T'))
      { daysOfWeek |= DaysOfWeek.Tuesday; }
      if (text.Contains('W'))
      { daysOfWeek |= DaysOfWeek.Wednesday; }
      if (text.Contains('R'))
      { daysOfWeek |= DaysOfWeek.Thursday; }
      if (text.Contains('F'))
      { daysOfWeek |= DaysOfWeek.Friday; }
      if (text.Contains('S'))
      { daysOfWeek |= DaysOfWeek.Saturday; }
      if (text.Contains('U'))
      { daysOfWeek |= DaysOfWeek.Sunday; }
      return daysOfWeek;
    }

    #endregion

  }
}
