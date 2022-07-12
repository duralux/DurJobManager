using System;

namespace DurJobManager.Jobs
{
  public interface IFileWatchJob : IJob
  {

    #region Properties

    string? Path { get; init; }
    string? Filter { get; init; }
    bool IncludeSubdirectories { get; init; }
    TimeSpan MinAge { get; init; }
    bool OnChange { get; init; }
    bool OnCreate { get; init; }
    bool OnDelete { get; init; }
    bool OnRename { get; init; }
    bool RunIfVanished { get; init; }

    #endregion

  }
}
