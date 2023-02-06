using Quartz;

namespace Unilevel.Jobs
{
    public class Test : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
