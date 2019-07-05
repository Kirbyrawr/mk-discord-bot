﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MK.Core;
using MK.Modules.BlackDesert.BossTimer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;

namespace MK.Modules.BlackDesert.BossTimer
{
    public class BossTimerModule : MKModule
    {
        public override string Name => "BDO World Boss Timer";

        public BossTimerConfig Config { get; private set; }

        private JObject _scheduleJSON;
        private Dictionary<string, Settings> _bossTimes;

        public struct Settings
        {
            public Color color;
            public List<TimeSchedule> schedule;
        }

        public struct TimeSchedule
        {
            public DayOfWeek weekDay;
            public int hour;
            public int minute;

            public TimeSchedule(DayOfWeek WeekDay, int Hour, int Minute)
            {
                weekDay = WeekDay;
                hour = Hour;
                minute = Minute;
            }
        }

        public override async Task Init()
        {
            GetConfigJSON();
            GetScheduleJSON();
            ParseJSON();
            await Run();
        }

        private void GetConfigJSON()
        {
            Log("Getting Config JSON");
            string configJSON = File.ReadAllText("src/MK.Modules/BlackDesert/BossTimer/Data/config.json");
            Config = JsonConvert.DeserializeObject<BossTimerConfig>(configJSON);
        }

        private void GetScheduleJSON()
        {
            Log("Getting Schedule JSON");
            string scheduleJSON = File.ReadAllText("src/MK.Modules/BlackDesert/BossTimer/Data/schedule.json");
            _scheduleJSON = JObject.Parse(scheduleJSON);
        }

        private void ParseJSON()
        {
            Log("Parsing JSON");

            _bossTimes = new Dictionary<string, Settings>();
            foreach (JObject boss in _scheduleJSON.GetValue("bosses").Children<JObject>())
            {
                Settings settings = new Settings();
                List<TimeSchedule> dateTimes = new List<TimeSchedule>();
                
                foreach (JProperty dayProperty in boss.GetValue("times").Children<JProperty>())
                {
                    JObject dayObject = (JObject)dayProperty.Value;

                    foreach (JToken hour in dayObject.GetValue("hours").Children())
                    {
                        DayOfWeek weekDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayProperty.Name);
                        string[] hourFormatted = hour.ToString().Split(':');

                        TimeSchedule dateTime = new TimeSchedule(weekDay, int.Parse(hourFormatted[0]), int.Parse(hourFormatted[1]));
                        dateTimes.Add(dateTime);
                    }
                }

                //Set Color
                uint colorValue = uint.Parse(boss.GetValue("color").ToString(), System.Globalization.NumberStyles.HexNumber);
                settings.color = new Color(colorValue);

                //Set Schedule
                settings.schedule = dateTimes;

                _bossTimes.Add(boss.GetValue("name").ToString(), settings);
            }
        }

        protected override async Task Run()
        {
            Log("Creating Cron Job");

            //Create Scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = await sf.GetScheduler();

            //Create Job
            IJobDetail job = JobBuilder.Create<BossTimerJob>()
                .WithIdentity("bossJob")
                .Build();

            //Set Data
            JobDataMap data = new JobDataMap();
            data.Add("bossTimes", _bossTimes);

            //Create Trigger
            ITrigger trigger = TriggerBuilder.Create()
            .UsingJobData(data)
            .WithCronSchedule("0 */5 * ? * *")
            .Build();

            //Schedule the Job
            await sched.ScheduleJob(job, trigger);

            // Start Scheduler
            await sched.Start();
        }

        public async void SendMessage(string bossName, double minutesLeft, DateTime bossTime, Color color)
        {
            ulong channelID = MKManager.GetInstance().GetModule<BossTimerModule>().Config.BossChannelID;
            SocketTextChannel channel = MKManager.GetInstance().client.GetChannel(channelID) as SocketTextChannel;

            EmbedBuilder embedBuilder = new EmbedBuilder();

            //Title
            embedBuilder.Title = bossName;

            //Color
            embedBuilder.Color = new Color();

            //Description
            if (minutesLeft == 0)
            {
                embedBuilder.AddField($"Live!", "", false);
            }
            else
            {
                embedBuilder.AddField($"Will appear in {minutesLeft}!", $"Spawn at {bossTime.Hour}:{bossTime.Minute}", false);
            }

            //Image
            string imagePath = $"src/MK.Modules/BlackDesert/BossTimer/Data/Images/{bossName.ToLower()}.png";
            embedBuilder.ThumbnailUrl = $"attachment://{bossName.ToLower()}.png";

            await channel.SendFileAsync(imagePath, "", false, embedBuilder.Build());
        }
    }
}