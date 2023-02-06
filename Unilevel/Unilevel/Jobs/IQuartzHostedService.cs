namespace Unilevel.Jobs
{
    public interface IQuartzHostedService
    {
        Task StartAsync();
        Task StopAsync();
        void SetUserId(string userId);
    }
}
