using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace DurJobManager.Jobs
{
  [DebuggerDisplay("Job: {Name,nq}, {Function,nq}, {Parameters.Count}")]
  public class GeneralJob : Job, IFileWatchJob, ITimedJob, IIntervalJob, IWebJob,
    IRunAtStartup
  {

    #region Properties

    #region RunAtStartupJob

    public bool RunAtStartup { get; init; } = false;

    #endregion


    #region FileWatchJob

    public string? Path { get; init; }
    public string? Filter { get; init; }
    public bool IncludeSubdirectories { get; init; } = false;
    public TimeSpan MinAge { get; init; }
    public bool OnChange { get; init; }
    public bool OnCreate { get; init; }
    public bool OnDelete { get; init; }
    public bool OnRename { get; init; }
    public bool RunIfVanished { get; init; }

    #endregion


    #region TimedJob

    public List<TimeSpan> Times { get; init; }

    #endregion


    #region Interval

    public TimeSpan Interval { get; init; }

    #endregion


    #region WebJob

    public string? WebModule { get; init; }
    public IPAddress? IPAddress { get; init; }
    public bool IsGet { get; init; }
    public bool IsPost { get; init; }
    public string? ContentPath { get; init; }
    public bool HasReturn { get; init; }
    public IEnumerable<string> ContentTypes { get; init; }

    #endregion

    #endregion


    #region Initialization

    public GeneralJob()
    {
      this.Times = new List<TimeSpan>();
      this.ContentTypes = new List<string>();
    }

    #endregion

  }
}
