using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DurJobManager.Jobs
{
  public class Manager
  {

    public const string NAME = "JobManager";

    #region Properties

    public List<GeneralJob> Jobs { get; set; }
    public int MaxConcurrentJobs { get; set; } = 1;
    public int MinConcurrentJobs { get; set; } = 1;
    public string Instance { get; set; } = "default";
    public TimeSpan QueueCheckInterval { get; set; } = TimeSpan.FromMilliseconds(25);
    public Dictionary<string, string> Parameters { get; set; }
    /// <summary>
    /// ServiceProvider, Function Parameters, Parameter Objects
    /// </summary>
    public Action<DurJobManager.Tasks.ITask?, IServiceProvider?, Dictionary<string, Type>?, Dictionary<string, object?>?>? PreAction { get; set; }
    /// <summary>
    /// ServiceProvider, Result, Parameter Objects
    /// </summary>
    public Action<DurJobManager.Tasks.ITask?, IServiceProvider?, Dictionary<string, Type>?, Dictionary<string, object?>?, object?>? PostAction { get; set; }

    public IEnumerable<GeneralJob> AllActiveJobs => this.Jobs
      .Where(j => j.IsActive);

    public IEnumerable<IRunAtStartup> RunAtStartup => this.Jobs
      .Where(j => j.IsRunAtStartup())
      .Cast<IRunAtStartup>();

    public IEnumerable<IFileWatchJob> FileWatchJobs => this.Jobs
      .Where(j => j.IsFileWatchJob())
      .Cast<IFileWatchJob>();

    public IEnumerable<IIntervalJob> IntervalJobs => this.Jobs
      .Where(j => j.IsIntervalJob())
      .Cast<IIntervalJob>();

    public IEnumerable<ITimedJob> TimedJobs => this.Jobs
      .Where(j => j.IsTimedJob())
      .Cast<ITimedJob>();

    public IEnumerable<IWebJob> WebJobs => this.Jobs
      .Where(j => j.IsWebJob())
      .Cast<IWebJob>();

    #endregion


    #region Properties

    public Manager()
    {
      this.Jobs = [];
      this.Parameters = [];
    }

    #endregion

  }
}
