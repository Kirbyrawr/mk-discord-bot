using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MK.Core
{
    public abstract class MKModule
    {
        public abstract string Name { get; }

        public abstract Task Init();
        protected abstract Task Run();

        protected void Log(string message)
        {
            string log = $"[{Name}] - {message}";
            Console.WriteLine(log);
        }
    }
}
