using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DurJobManager.Jobs;
using DurJobManager.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DurJobManager.Worker
{
  public class TimerQueueWorker : BackgroundService
  {

    #region Properties

    private readonly Jobs.Manager _jobManager;
    private readonly BackgroundQueue _backgroundQueue;

    #endregion


    #region Initialization

    public TimerQueueWorker(IOptions<Jobs.Manager> options,
      BackgroundQueue backgroundQueue)
    {
      this._jobManager = options.Value;
      this._backgroundQueue = backgroundQueue;
    }

    #endregion


    #region Functions

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
      if (!_jobManager.TimedJobs.Any())
      { return; }

      if (this._backgroundQueue == null)
      { return; }

      await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

      while (!cancellationToken.IsCancellationRequested)
      {
        var jobs = this._jobManager.TimedJobs
          .SelectMany(j => j.Times.Select(t => new { Time = t, Job = j }));

        // Only show jobs not run today
        var jobsFilterd = jobs
          .Where(j => DateTime.Now.TimeOfDay <= j.Time);

        if (jobsFilterd.Any())
        { jobs = jobsFilterd; }

        jobs = [.. jobs.OrderBy(j => j.Time)];

        var firstTime = jobs.First().Time;
        var waitTime = firstTime < DateTime.Now.TimeOfDay ?
          DateTime.Now.AddDays(1).Date - DateTime.Now :
          firstTime - DateTime.Now.TimeOfDay;

        await Task.Delay(waitTime, cancellationToken);
        foreach (var jobGroup in jobs.Where(j => j.Time == firstTime))
        {
          if (jobGroup.Job.IsNowActive())
          {
            this._backgroundQueue.Enqueue(new Tasks.PSTask(jobGroup.Job));
          }
        }
      }
    }

    #endregion

  }
}
