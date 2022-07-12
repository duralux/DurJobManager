using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DurJobManager.Jobs;
using DurJobManager.Service;
using DurJobManager.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DurJobManager.Worker
{
  public class IntervalQueueWorker : BackgroundService
  {

    #region Properties

    private readonly Jobs.Manager _jobManager;
    private readonly BackgroundQueue _backgroundQueue;
    private readonly List<System.Timers.Timer> _timers;

    #endregion


    #region Initialization

    public IntervalQueueWorker(IOptions<Jobs.Manager> options,
      BackgroundQueue backgroundQueue)
    {
      this._jobManager = options.Value;
      this._backgroundQueue = backgroundQueue;

      this._timers = new List<System.Timers.Timer>();
    }

    #endregion


    #region Functions

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
      if (!_jobManager.IntervalJobs.Any())
      { return; }

      if (this._backgroundQueue == null)
      { return; }

      await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

      foreach (var job in this._jobManager.IntervalJobs)
      {
        var timer = new System.Timers.Timer
        {
          Interval = job.Interval.TotalMilliseconds,
          AutoReset = true,
          Enabled = true
        };
        timer.Elapsed += (sender, e) => RunTask(job);
        timer.Start();

        this._timers.Add(timer);

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
      }
      await Task.Delay(-1, cancellationToken);
    }


    public void RunTask(IIntervalJob job)
    {
      if (job.IsNowActive())
      {
        this._backgroundQueue.Enqueue(new PSTask(job));
      }
    }


    #endregion

  }
}
