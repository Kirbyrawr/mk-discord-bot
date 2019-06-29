using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MK
{
    public class MKManager
    {
        private static MKManager _instance;

        public MKData data;
        public DiscordSocketClient client;

        public MKManager()
        {
            SetInstance();
            Setup();
        }

        public async Task Init()
        {
            await client.LoginAsync(TokenType.Bot, data.Config.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private void SetInstance()
        {
            _instance = this;
        }

        public static MKManager GetInstance()
        {
            return _instance;
        }

        private void Setup()
        {
            data = new MKData();
            client = new DiscordSocketClient();
            client.Ready += OnReady;
            client.Disconnected += OnDisconnected;
        }

        //Events
        private Task OnReady()
        {
            Console.WriteLine("MK - Ready");
            return Task.CompletedTask;
        }

        private Task OnDisconnected(Exception e)
        {
            Console.WriteLine("MK - Disconnected");
            return Task.CompletedTask;
        }
    }
}
