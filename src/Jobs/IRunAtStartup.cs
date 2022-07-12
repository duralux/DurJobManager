namespace DurJobManager.Jobs
{
  public interface IRunAtStartup : IJob
  {

    #region Properties

    bool RunAtStartup { get; init; }

    #endregion

  }
}
