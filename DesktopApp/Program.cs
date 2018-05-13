using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace DesktopApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            SerialPort port = new SerialPort("COM4", 115200);
            port.Open();
            //port.ReadTimeout = 5000;
           // port.WriteTimeout = 5000;
            Console.WriteLine("Connected");

            while (true)
            {
                bool isAl = false;
                bool isRdy = false;
                Console.WriteLine("Starting con");
                isAlive(port, isAl);
                isReady(port, isRdy);
                sendData(port, BitConverter.GetBytes(i));
                Thread.Sleep(5000);
                i++;
            }
            //port.Close();

        }
       static void isAlive(SerialPort port, bool isAl)
        {
            while (!isAl) {
                port.Write(new byte[] { 0x01, 0x01, 0x01 }, 0, 3);
                Thread.Sleep(5000);
                Console.WriteLine("Send isAlive");
                byte[] byt = new byte[1];
                Thread.Sleep(5000);
                port.Read(byt, 0, 1);
                Console.WriteLine("IREAD");
                if (byt[0] == 0x01)
                {
                    isAl = true;
                    Console.WriteLine("ESP is Alive");
                }
                else if(byt[0]==0x03)
                {
                    Console.WriteLine("Ërror detected");
                }
                else
                {
                    Console.Write(byt[0]);
                }
            }
        }
        static void isReady(SerialPort port, bool isRdy)
        {
            while (!isRdy)
            {
                port.Write(new byte[] { 0x02, 0x02, 0x02 }, 0, 3);
                Console.WriteLine("Send isERdy");
                byte[] byt = new byte[1];
                port.Read(byt, 0, 1);
                if (byt[0] == 0x02)
                {
                    isRdy = true;
                    Console.WriteLine("ESP is Ready");
                }
                else if (byt[0] == 0x03)
                {
                    Console.WriteLine("Error Detected");
                    isAlive(port, false);
                }
            }
        }
        static void sendData(SerialPort port, byte[] data)
        {
            port.Write(data, 0, data.Count());
            byte[] byt = new byte[1];
            port.Read(byt, 0, 1);
            if (byt[0] == 0x03)
            {
                Console.WriteLine("Error");
            }
            else if (byt[0] == 0x10)
            {
                Console.WriteLine("Done");
            }

        }
    }
}
