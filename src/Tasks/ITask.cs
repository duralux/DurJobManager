using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DurJobManager.Tasks
{
  public interface ITask
  {

    #region Properties

    int EventID { get; set; }
    string Command { get; set; }
    TimeSpan Timeout { get; set; }
    string? Name { get; set; }
    object? Result { get; set; }
    TimeSpan Runtime { get; set; }
    EventWaitHandle? EventWaitHandle { get; set; }
    bool ErrorOnCancel { get; set; }

    #endregion


    #region Functions

    Task RunAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken);

    #endregion

  }
}
