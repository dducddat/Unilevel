using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Unilevel.Jobs
{
    public class QuartzHostedService : IHostedService
    {
        private IScheduler scheduler;

        private readonly IJobFactory _jobFactory;

        private string UserId;

        public QuartzHostedService(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();
            scheduler.JobFactory = _jobFactory;

            
            var job = CreateJob(UserId);
            var trigger = CreateTrigger(UserId);
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await scheduler.Shutdown();
        }

        private static IJobDetail CreateJob(string userId)
        {
            return JobBuilder
                .Create<Test>()
                .WithIdentity(userId, "VisitPlan")
                .Build();
        }

        private static ITrigger CreateTrigger(string userId)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity(userId, "VisitPlan")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).WithRepeatCount(5))
                .StartNow()
                .Build();
        }

        public void SetUserId(string userId)
        {
            UserId = userId;
        }
    }
}
