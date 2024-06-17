using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurJobManager.Service
{
  public class BackgroundQueue
  {

    #region Properties

    private readonly ILoggerFactory _loggerFactory;
    private readonly Jobs.Manager _jobManager;
    private readonly IServiceProvider _serviceProvider;
    private int _concurrentCount;

    internal int ConcurrentCount => this._concurrentCount;
    internal ConcurrentQueue<Tasks.ITask> TaskQueue { get; private set; }

    internal int MaxConcurrentCount => this._jobManager.MaxConcurrentJobs;
    internal TimeSpan QueueCheckInterval => this._jobManager.QueueCheckInterval;

    public TimeSpan ConsumedTime { get; private set; }
    public DateTime StartTime { get; init; }

    #endregion


    #region Initialization

    public BackgroundQueue(ILoggerFactory loggerFactory,
      IOptions<Jobs.Manager> manager, IServiceProvider serviceProvider)
    {
      this._loggerFactory = loggerFactory;
      this._jobManager = manager.Value;
      this._serviceProvider = serviceProvider;

      this.TaskQueue = new ConcurrentQueue<Tasks.ITask>();
      this.StartTime = DateTime.Now;
    }

    #endregion


    #region Functions

    public void Enqueue(string command, Dictionary<string, object?> parameters = null!)
    {
      TaskQueue.Enqueue(new Tasks.PSTask(command, parameters));
    }


    public void Enqueue(Tasks.ITask task)
    {
      TaskQueue.Enqueue(task);
    }


    internal async Task Dequeue(CancellationToken cancellationToken)
    {
      if (TaskQueue.TryDequeue(out var task))
      {
        string categoryName = "DurWorker.Tasks" + (task.Name == null ? "" : $".{task!.Name}");
        var logger = this._loggerFactory.CreateLogger(categoryName);
        var start = DateTime.Now;

        Interlocked.Increment(ref this._concurrentCount);
        try
        {
          var timeoutTokenSource = new CancellationTokenSource(task.Timeout);
          using var linkedCancellation = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

          await task.RunAsync(this._serviceProvider, logger, timeoutTokenSource.Token);
        } catch (OperationCanceledException ex)
        {
          logger.Log(task.ErrorOnCancel ? LogLevel.Error : LogLevel.Information,
            new EventId(1), ex, $"Opeartion canceled: {task.Command}");
        }
        catch (Exception ex)
        {
          logger.Log(task.ErrorOnException ? LogLevel.Error : LogLevel.Information, 
            task.EventID, ex, $"Error with Job: {task.Command}");
        }
        finally
        {
          this.ConsumedTime += DateTime.Now - start;
          Interlocked.Decrement(ref this._concurrentCount);
          GC.Collect();
        }
      }

      await Task.CompletedTask;
    }

    #endregion

  }
}
