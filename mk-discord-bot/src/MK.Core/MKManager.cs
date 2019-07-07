using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MK.Models;
using Newtonsoft.Json;

namespace MK.Core
{
    public class MKManager
    {
        public DiscordSocketClient client;

        private static MKManager _instance;
        private MKConfig _config;
        private Dictionary<Type, MKModule> _modules;
        private bool _modulesCreated;

        public MKManager()
        {
            SetInstance();
            Setup();
        }

        //Instance
        private void SetInstance()
        {
            _instance = this;
        }

        public static MKManager GetInstance()
        {
            return _instance;
        }

        //Setup
        private void Setup()
        {
            SetupLog();
            LoadConfig();
            CreateClient();
        }

        private void SetupLog()
        {
            //Make a copy of the previous log.
            if (File.Exists("Log.txt"))
            {
                File.Copy("Log.txt", "LastLog.txt", true);
                File.WriteAllText("Log.txt", "");
            }
            else
            {
                File.Create("Log.txt").Close();
            }
        }

        private void LoadConfig()
        {
            Log("Loading Config");
            string configJSON = File.ReadAllText("src/MK.Core/Data/config.json");
            _config = JsonConvert.DeserializeObject<MKConfig>(configJSON);
        }

        private void CreateClient()
        {
            Log("Creating Discord Client");
            client = new DiscordSocketClient();
            client.Ready += OnReady;
            client.Disconnected += OnDisconnected;
        }

        public async Task Init()
        {
            await client.LoginAsync(TokenType.Bot, _config.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        //Modules
        private void CreateModules()
        {
            if(_modulesCreated) { return; }

            Log("Create Modules");

            _modules = new Dictionary<Type, MKModule>();

            foreach (Type t in Assembly.GetEntryAssembly().GetTypes())
            {
                if(t.IsSubclassOf(typeof(MKModule)) && t.FullName != typeof(MKModule).FullName)
                {
                    MKModule module = (MKModule)Activator.CreateInstance(t);
                    module.Init();

                    _modules.Add(t, module);
                }
            }

            _modulesCreated = true;
        }

        public T GetModule<T>() where T : MKModule
        {
            _modules.TryGetValue(typeof(T), out MKModule module);
            return (T)module;
        }

        //Events
        private Task OnReady()
        {
            Log("Ready");
            CreateModules();
            return Task.CompletedTask;
        }

        private Task OnDisconnected(Exception e)
        {
            Log("Disconnected");
            return Task.CompletedTask;
        }

        //Debug
        private void Log(string message)
        {
            string log = $"[MK Core] - {message}";
            Console.WriteLine(log);

            //Write to log file.
            File.AppendAllText("Log.txt", $"[{DateTime.Now}] {message}{Environment.NewLine}");
        }
    }
}