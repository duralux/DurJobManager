using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DurJobManager.Jobs;
using DurJobManager.Service;
using DurJobManager.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurJobManager.Worker
{
  class FileWatchQueueWorker : BackgroundService
  {

    #region Properties

    private readonly Jobs.Manager _jobManager;
    private readonly BackgroundQueue _backgroundQueue;
    private readonly ILogger<FileWatchQueueWorker> _logger;
    private readonly List<System.IO.FileSystemWatcher> _fsw;

    #endregion


    #region Initialization

    public FileWatchQueueWorker(IOptions<Jobs.Manager> options,
      BackgroundQueue backgroundQueue, ILogger<FileWatchQueueWorker> logger)
    {
      this._jobManager = options.Value;
      this._backgroundQueue = backgroundQueue;
      this._logger = logger;

      this._fsw = [];
    }

    #endregion


    #region Functions

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
      if (!_jobManager.FileWatchJobs.Any())
      { return; }

      if (this._backgroundQueue == null)
      { return; }

      await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

      foreach (var job in this._jobManager.FileWatchJobs)
      {
        var fsw = new System.IO.FileSystemWatcher
        {
          Path = job.Path!,
          IncludeSubdirectories = job.IncludeSubdirectories
        };
        fsw.Error += (sender, e) => OnError(job, e);

        if (job.OnChange)
        {
          fsw.Changed += async (sender, e) => await RunTask(job, e);
        }
        if (job.OnCreate)
        {
          fsw.Created += async (sender, e) => await RunTask(job, e);
        }
        if (job.OnDelete)
        {
          fsw.Deleted += async (sender, e) => await RunTask(job, e);
        }
        if (job.OnRename)
        {
          fsw.Renamed += async (sender, e) => await RunTask(job, e);
        }
        if (job.Filter != null)
        {
          fsw.Filter = job.Filter;
        }
        fsw.EnableRaisingEvents = true;

        this._fsw.Add(fsw);
      }
      await Task.Delay(-1, cancellationToken);
    }


    public void OnError(IFileWatchJob job, System.IO.ErrorEventArgs e)
    {
      this._logger.LogError($"Error on filewatch! {job.Function} -> {e.GetException()}");
    }


    public async Task RunTask(IFileWatchJob job, System.IO.FileSystemEventArgs e)
    {
      if (!job.IsNowActive())
      { return; }

      this._logger.LogInformation($"Start job '{job.Name}' from event {e.ChangeType} on file: {e.FullPath}");
      await Task.Delay(job.MinAge);

      if (e.ChangeType == System.IO.WatcherChangeTypes.Deleted &&
        !(job.RunIfVanished || System.IO.File.Exists(e.FullPath)))
      {
        this._logger.LogInformation($"Nothing to do for {e.FullPath}");
        return;
      }
      this._logger.LogInformation($"Start job for {e.FullPath}");

      var task = new PSTask(job);
      task.Parameters["changeType"] = e.ChangeType;
      task.Parameters["filepath"] = e.FullPath;
      task.Parameters["name"] = e.Name;
      if (e is System.IO.RenamedEventArgs re)
      {
        task.Parameters["oldfilepath"] = re.OldFullPath;
        task.Parameters["oldname"] = re.OldName;
      }

      this._backgroundQueue.Enqueue(task);
    }


    #endregion

  }
}
