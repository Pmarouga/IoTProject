using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Diagnostics;

namespace Server
{
    class Program
    {
        // private static AutoResetEvent event_1 = new AutoResetEvent(false);
        static int thing = 125;
        static int timeInt = 121;
        public static string timeInterval;

        static void Main(string[] args)
        {
            var uri = new Uri("http://192.168.2.2:80");

            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;
            using (var host = new NancyHost(hostConfigs, uri))
            {
                host.Start();
                Console.WriteLine("Your application is running on " + uri);
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
            Console.ReadKey();
        }
    }
}








