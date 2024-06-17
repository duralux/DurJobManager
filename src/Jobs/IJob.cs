using System;
using System.Collections.Generic;

namespace DurJobManager.Jobs
{
  public interface IJob
  {

    #region Properties

    int EventID { get; init; }
    JobType JobType {  get; init; }
    string? Function { get; init; }
    TimeSpan Timeout { get; init; }
    Dictionary<string, string> Parameters { get; init; }
    string? Description { get; init; }
    string? Name { get; init; }
    bool IsActive { get; init; }
    bool ErrorOnCancel { get; init; }
    bool ErrorOnException { get; init; }

    DateTime? StartDate { get; init; }
    DateTime? StopDate { get; init; }

    TimeSpan? StartDayTime { get; init; }
    TimeSpan? StopDayTime { get; init; }
    public DaysOfWeek DaysOfWeek { get; }
    string? WorkDays { get; init; }

    #endregion

  }
}
