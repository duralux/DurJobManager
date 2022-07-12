using System;
using System.Linq;
using DurJobManager.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DurJobManager
{
  public static class WorkerExtension
  {

    #region Functions

    public static IServiceCollection AddJobManager(this IServiceCollection services,
      HostBuilderContext? hostContext = null!)
    {
      if (hostContext != null)
      {

      }

      services.Configure<Jobs.Manager>(jobManager =>
      {
        var names = jobManager.Jobs
          .Where(j => j.IsActive)
          .Where(j => j.Name != null)
          .GroupBy(k => k.Name, v => v)
          .Where(n => n.AsEnumerable().Count() > 1)
          .ToList();

        if (names.Count > 0)
        {
          throw new ArgumentException(
            $"Jobs must have diffrent names: '{names.First().Key}' is defined multiple times!");
        }
      });

      services.AddSingleton<Service.BackgroundQueue>();

      services.AddHostedService<RunQueueWorker>();
      services.AddHostedService<RunAtStartupQueueWorker>();
      services.AddHostedService<TimerQueueWorker>();
      services.AddHostedService<IntervalQueueWorker>();
      services.AddHostedService<FileWatchQueueWorker>();

      return services;
    }

    #endregion

  }
}
