using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DurJobManager.Tasks
{
  [DebuggerDisplay("{Name,nq}, {Command,nq}, {EventID, nq}")]
  public class CommandTask : ITask
  {

    #region Properties

    public int EventID { get; set; }
    public string Command { get; set; }
    public string? Arguments { get; set; }
    public bool Wait { get; set; }
    public TimeSpan Timeout { get; set; }
    public string? Name { get; set; }
    public object? Result { get; set; }
    public TimeSpan Runtime { get; set; }
    public System.Threading.EventWaitHandle? EventWaitHandle { get; set; }
    public bool ErrorOnCancel { get; set; }
    public bool ErrorOnException { get; set; }

    #endregion


    #region Initialization

    public CommandTask(string command, string? arguments = null!, bool wait = false)
    {
      this.Command = command;
      this.Arguments = arguments;
      this.Wait = wait;
    }


    public CommandTask(Jobs.IJob job) :
      this(job.Function!)
    {
      this.Name = job.Name;
      this.EventID = job.EventID;
      this.Timeout = job.Timeout;

      if (job.JobType != Jobs.JobType.PowerShell)
      {
        throw new ArgumentException($"{nameof(CommandTask)} can only be derived from {nameof(Jobs.JobType.CMD)} Job", nameof(job.JobType));
      }
    }

    #endregion


    #region Function

    public async Task RunAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
      var p = new System.Diagnostics.Process();

      p.StartInfo.FileName = this.Command;
      if(this.Arguments != null)
      {
        p.StartInfo.Arguments = this.Arguments;
      }

      var watch = new Stopwatch();
      watch.Start();
      p.Start();

      if(this.Wait)
      {
        await p.WaitForExitAsync(cancellationToken);
        watch.Stop();
        this.Runtime = watch.Elapsed;

        if (this.EventWaitHandle != null)
        {
          this.EventWaitHandle.Set();
        }
      }

      await Task.CompletedTask;
    }

    #endregion

  }
}
