using System.Threading;
using System.Threading.Tasks;
using DurJobManager.Service;
using Microsoft.Extensions.Hosting;

namespace DurJobManager.Worker
{
  public class RunQueueWorker : BackgroundService
  {

    #region Properties

    private readonly BackgroundQueue _backgroundQueue;

    #endregion


    #region Initialization

    public RunQueueWorker(BackgroundQueue backgroundQueue)
    {
      this._backgroundQueue = backgroundQueue;
    }

    #endregion


    #region Function

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        if (_backgroundQueue.TaskQueue.IsEmpty ||
          this._backgroundQueue.ConcurrentCount > this._backgroundQueue.MaxConcurrentCount)
        {
          await Task.Delay(_backgroundQueue.QueueCheckInterval, cancellationToken);
        }
        else
        {
          await _backgroundQueue.Dequeue(cancellationToken);
        }
      }
    }

    #endregion

  }
}
