using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DurJobManager.Tasks
{
  [DebuggerDisplay("{Name,nq}, {Command,nq}, {EventID, nq}")]
  public class PSTask : ITask
  {

    #region Properties

    public int EventID { get; set; }
    public string Command { get; set; }
    public TimeSpan Timeout { get; set; }
    public Dictionary<string, object?> Parameters { get; set; }
    public string? Name { get; set; }
    public object? Result { get; set; }
    public TimeSpan Runtime { get; set; }
    public EventWaitHandle? EventWaitHandle { get; set; }
    public bool ErrorOnCancel { get; set; }
    public bool ErrorOnException { get; set; }

    #endregion


    #region Initialization

    public PSTask(string command, Dictionary<string, object?>? parameters = null!)
    {
      this.Command = command ?? throw new ArgumentNullException(nameof(this.Command),
        "Command must not be null!");
      this.Parameters = parameters?
        .ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase) ??
        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
      this.Timeout = TimeSpan.FromSeconds(10);
    }


    public PSTask(Jobs.IJob job) :
      this(job.Function!, job.Parameters.ToDictionary(k => k.Key, v => (object?)v.Value,
          StringComparer.OrdinalIgnoreCase))
    {
      this.Name = job.Name;
      this.EventID = job.EventID;
      this.Timeout = job.Timeout;
      this.ErrorOnCancel = job.ErrorOnCancel;
      this.ErrorOnException = job.ErrorOnException;

      if (job.JobType != Jobs.JobType.PowerShell)
      {
        throw new ArgumentException($"{nameof(PSTask)} can only be derived from {nameof(Jobs.JobType.PowerShell)} Job", nameof(job.JobType));
      }
    }

    #endregion


    #region Functions

    public async Task RunAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken = default)
    {
      DurHostedRunspace.HostedRunspace? rs = null!;
      Jobs.Manager? manager = null!;
      var watch = new Stopwatch();

      try
      {
        rs = serviceProvider
          .GetService<DurHostedRunspace.HostedRunspaceService>()!
          .GetHostedRunspace(logger);
        manager = serviceProvider
          .GetService<IOptions<Jobs.Manager>>()?
          .Value;

        manager?.PreAction?.Invoke(this, serviceProvider, rs.GetParametersFromFunction(this.Command), this.Parameters);

        watch.Start();
        this.Result = (await rs
          .RunCommandAsync(this.Command, this.Parameters!, true, cancellationToken));
        watch.Stop();

        this.Runtime = watch.Elapsed;
      } catch
      {
        throw;
      } finally
      {
        this.EventWaitHandle?.Set();
        manager?.PostAction?.Invoke(this, serviceProvider, rs.GetParametersFromFunction(this.Command), this.Parameters, this.Result);

        var logLevel = (LogLevel)(Math.Max((int)LogLevel.Information, (int)(rs?.MaxLogLevel ?? 0)));
        logger.Log(logLevel,
          this.EventID, ($"[{this.Command}] " +
          $"Finished in {watch.Elapsed:hh\\:mm\\:ss\\.ffff}" +
          Environment.NewLine + rs?.Log?.ToString()).Trim());
      }
    }


    public override string ToString()
    {
      return $"{this.Name} [{this.Command}]".Trim();
    }

    #endregion

  }
}
