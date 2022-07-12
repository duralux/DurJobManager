using System;

namespace DurJobManager.Jobs
{
  public static class JobExtensions
  {

    #region Static Function

    public static bool IsNowActive(this IJob job)
    {
      var now = DateTime.Now;
      return job.IsActive &&
        (job.StartDate == null || job.StartDate <= now) &&
        (job.StopDate == null || now <= job.StopDate) &&
        (job.StartDayTime == null || job.StartDayTime <= now.TimeOfDay) &&
        (job.StopDayTime == null || now.TimeOfDay <= job.StopDayTime) &&
        job.DaysOfWeek.HasDay(now.DayOfWeek);
    }


    public static bool IsRunAtStartup(this IRunAtStartup job)
    {
      return job.Function != null && job.RunAtStartup;
    }


    public static bool IsFileWatchJob(this IFileWatchJob job)
    {
      return job.Function != null && job.Path != null && job.Path.Length > 0 &&
      (job.OnChange || job.OnCreate || job.OnDelete || job.OnRename);
    }


    public static bool IsIntervalJob(this IIntervalJob job)
    {
      return job.Function != null && job.Interval > TimeSpan.Zero &&
        job.Interval != System.Threading.Timeout.InfiniteTimeSpan;
    }


    public static bool IsTimedJob(this ITimedJob job)
    {
      return job.Function != null && job.Times.Count > 0;
    }


    public static bool IsWebJob(this IWebJob job)
    {
      return job.WebModule != null && job.WebModule.Length > 0 &&
        (job.IsGet || job.IsPost) && job.Timeout >= TimeSpan.Zero;
    }

    #endregion

  }
}
