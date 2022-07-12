using System.Collections.Generic;

namespace DurJobManager.Jobs
{
  public interface IWebJob : IJob
  {

    #region Properties

    public string? WebModule { get; init; }
    public System.Net.IPAddress? IPAddress { get; init; }
    public bool IsGet { get; init; }
    public bool IsPost { get; init; }
    public string? ContentPath { get; init; }
    public IEnumerable<string> ContentTypes { get; init; }
    public bool HasReturn { get; init; }

    #endregion

  }
}
