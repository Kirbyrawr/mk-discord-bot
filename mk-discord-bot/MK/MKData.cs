using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MK.Models;
using Newtonsoft.Json;

namespace MK
{
    public class MKData
    {
        public MKConfig Config { get; private set; }
        public string FolderPath { get; private set; }

        public MKData()
        {
            Setup();
        }

        private void Setup()
        {
            SetFolderPath();
            LoadConfig();
        }

        private void SetFolderPath()
        {
            FolderPath = Environment.CurrentDirectory;
        }

        private void LoadConfig()
        {
            string json = File.ReadAllText(GetFilePathRelative("config.json"));
            Config = JsonConvert.DeserializeObject<MKConfig>(json);
        }

        public string GetFilePathRelative(string filePath)
        {
            return Path.Combine(FolderPath, filePath);
        }
    }
}
