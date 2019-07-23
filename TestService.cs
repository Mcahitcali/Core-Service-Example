using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Core_Service_Example
{
    public class TestService
    {
        private static TimeSpan DelayJob = TimeSpan.FromSeconds(10);
    public void Start()
    {
        
        InitializeJobs().GetAwaiter().GetResult();
    }

    public void Stop()
    {
    }

    private static async Task InitializeJobs()
    {
        var _scheduler = await new StdSchedulerFactory().GetScheduler();
        await _scheduler.Start();

        var rasyoServiceJob = JobBuilder.Create<ServiceJob>()
          .WithIdentity("RasyoJob","RasyoTek")
          .Build();

        var rasyoServiceTrigger = TriggerBuilder.Create()
          .WithIdentity("RasyoJob","RasyoTek")
          .StartNow()
          .WithSimpleSchedule(x=>
                                x.WithInterval(DelayJob)
                                .RepeatForever())
          .Build();

        await _scheduler.ScheduleJob(rasyoServiceJob, rasyoServiceTrigger);
    }
    }
}