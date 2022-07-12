using System;

namespace DurJobManager.Jobs
{
  public interface IIntervalJob : IJob
  {

    #region Properties

    TimeSpan Interval { get; init; }

    #endregion

  }
}
