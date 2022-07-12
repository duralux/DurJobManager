using System;
using System.Collections.Generic;

namespace DurJobManager.Jobs
{
  public interface ITimedJob : IJob
  {

    #region Properties

    List<TimeSpan> Times { get; init; }

    #endregion

  }
}
