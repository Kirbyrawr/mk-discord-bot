using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MK.Core;
using Quartz;

namespace MK.Modules.BlackDesert.BossTimer
{
    public class BossTimerJob : IJob
    {
        public virtual Task Execute(IJobExecutionContext context)
        {     
            Dictionary<string, BossTimerModule.Settings> bossTimes = (Dictionary <string, BossTimerModule.Settings>)context.MergedJobDataMap.Get("bossTimes");

            DateTime dateNow = DateTime.Now;

            foreach (var boss in bossTimes)
            {
                foreach (var time in boss.Value.schedule)
                {
                    DateTime dateBoss = new DateTime(dateNow.Year, dateNow.Month, dateNow.ClosestWeekDay(time.weekDay).Day, time.hour, time.minute, 0);
                    double minutesLeft = (dateBoss - dateNow).TotalMinutes;

                    if (minutesLeft >= 0 && minutesLeft <= 20)
                    {
                        int minutesFormatted = (int)Math.Round(minutesLeft);
                        MKManager.GetInstance().GetModule<BossTimerModule>().SendMessage(boss.Key, minutesFormatted, dateBoss, boss.Value.color);
                    }
                }
            }
            
            return Task.CompletedTask;
        }
    }
}