using System;
using System.Threading;

namespace MK
{
    class Program
    {
        private static void Main(string[] args)
        {
            new MKManager().Init().GetAwaiter().GetResult();
        }
    }
}
