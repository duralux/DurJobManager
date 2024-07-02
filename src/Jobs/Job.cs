using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DurJobManager.Jobs
{
  [DebuggerDisplay("Job: {Function,nq}, {Parameters.Count}")]
  public abstract class Job
  {

    #region Properties

    public int EventID { get; init; }
    public JobType JobType {  get; init; }
    public string? Function { get; init; }
    public TimeSpan Timeout { get; init; }
    public Dictionary<string, string> Parameters { get; init; }
    public string? Description { get; init; }
    public string? Name { get; init; }
    public bool IsActive { get; init; }
    public bool ErrorOnCancel { get; init; }
    public bool ErrorOnException { get; init; }

    public DateTime? StartDate { get; init; }
    public DateTime? StopDate { get; init; }
    public TimeSpan? StartDayTime { get; init; }
    public TimeSpan? StopDayTime { get; init; }

    public DaysOfWeek DaysOfWeek => this.WorkDays.GetDaysOfWeek();
    public string? WorkDays { get; init; }

    #endregion


    #region Initialization

    public Job()
    {
      this.JobType = JobType.PowerShell;
      this.Timeout = TimeSpan.FromSeconds(60);
      this.Parameters = [];
      this.IsActive = true;
      this.ErrorOnCancel = true;
      this.EventID = 1;
    }

    #endregion

  }
}
