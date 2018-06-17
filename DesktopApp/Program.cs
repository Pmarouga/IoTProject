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
        static DateTime start;
        static TimeSpan time;
        static bool iSent = false;
        static bool isAl = false;
        static bool isRdy = false;
        static bool iRead = false;
        static int i = 11;
        static void Main(string[] args)
        {
            SerialPort port = new SerialPort("COM6", 115200);

            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            port.Open();
            port.WriteTimeout = 5000;
            Console.WriteLine("Connected");

            start = DateTime.Now;
            while (true)
            {
                if (!isAl)
                {
                    if (!iSent)
                    {
                        isAlive(port);
                    }

                }
                else if (!isRdy)

                {
                    if (!iSent)
                    {
                        isReady(port);
                    }
                }
                else
                {
                    if (!iSent)
                    { 
                        sendData(port, BitConverter.GetBytes(i));
                        i++;   
                    }
                }
                time = DateTime.Now - start;
                if(iSent && !iRead  && time.Seconds > 10)
                {
                    isAl=false;
                    isRdy=false;
                    iSent = false;    
                }   
            }
        }
       static void reset()
        {
            isAl = false;
            isRdy = false;
            iSent = false;
            iRead = false;
        }
       static  void isAlive(SerialPort port)
        {
            
                start = DateTime.Now;
                iSent = true;
                iRead=false;
                port.Write(new byte[] { 0x07, 0x07, 0x07 }, 0, 3);
                Console.WriteLine("Send isAlive");                         
        }
        static void isReady(SerialPort port)
        {
                start = DateTime.Now;
                iSent = true;
                iRead = false;
                port.Write(new byte[] { 0x02, 0x02, 0x02 }, 0, 3); 
                Console.WriteLine("Send isRdy");
        }
        static void sendData(SerialPort port, byte[] data)
        {
            start = DateTime.Now;
            iSent = true;
            iRead = false;
            port.Write(data, 0, data.Count());
            Console.WriteLine("Sent Data");
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
                byte[] b=new byte[1];
            int indata = sp.ReadByte();
            Console.WriteLine("Data Received:");
            Console.Write(indata);
            iRead = true;
            if (isAl == false)
            {
                if (indata == 0x07)
                {
                    isAl = true;
                    iSent = false;
                }
                else if (indata == 0x03)
                {
                    Console.WriteLine("Failed isAl");
                    iSent = false;
                }
                else
                {
                    Console.WriteLine("failed isAL with " + indata);
                    iSent = false;
                }
            }
            else if (isRdy == false)
            {
                if (indata == 0x02)
                {
                    isRdy = true;
                    iSent = false;
                }
                else if (indata == 0x03)
                {
                    Console.WriteLine("Failed isrdy");
                }
                else
                {
                    Console.WriteLine("failed isrdy with " + indata);
                }
            }
            else
            {
                if (indata == 0x09)
                {
                    Console.WriteLine("Data Sent Success");
                }
                else if (indata == 0x03)
                {
                    Console.WriteLine("Failed Data Sent");
                }
                else
                {
                    Console.Write("Failed sent data with "+indata +"when state is"+i + "and Count"+ BitConverter.GetBytes(i).Count());
                }
                reset();
            }
           
        }
    }
}
