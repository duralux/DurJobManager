using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DurJobManager.Tasks
{
  public class NativeTask : ITask
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

    #endregion


    #region Initialization

    public NativeTask(string command, string? arguments = null!, bool wait = false)
    {
      this.Command = command;
      this.Arguments = arguments;
      this.Wait = wait;
    }


    public NativeTask(Jobs.IJob job) :
      this(job.Function!)
    {
      this.Name = job.Name;
      this.EventID = job.EventID;
      this.Timeout = job.Timeout;

      if (job.JobType != Jobs.JobType.Nativ)
      {
        throw new ArgumentException($"{nameof(NativeTask)} can only be derived from {nameof(Jobs.JobType.Nativ)} Job", nameof(job.JobType));
      }
    }

    #endregion


    #region Function

    public async Task RunAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
      throw new NotImplementedException(await Task.FromResult(""));
    }

    #endregion

  }
}
