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
  public class RunAtStartupQueueWorker : BackgroundService
  {

    #region Properties

    private readonly Jobs.Manager _jobManager;
    private readonly BackgroundQueue _backgroundQueue;

    #endregion


    #region Initialization

    public RunAtStartupQueueWorker(IOptions<Jobs.Manager> options,
      BackgroundQueue backgroundQueue)
    {
      this._jobManager = options.Value;
      this._backgroundQueue = backgroundQueue;
    }

    #endregion


    #region Functions

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
      if (!_jobManager.RunAtStartup.Any())
      { return; }

      if (this._backgroundQueue == null)
      { return; }

      foreach (var job in this._jobManager.RunAtStartup)
      {
        if (job.IsNowActive())
        {
          this._backgroundQueue.Enqueue(new PSTask(job));
        }
      }

      await Task.Delay(0, cancellationToken);
    }

    #endregion

  }
}
