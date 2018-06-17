using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.IO;
using System.Net.Http;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Server
{
    public class RestModule : NancyModule
    {
        public static string AuthToken = "lVDd51borSM66TdS5Ucr1rGNEAveCP";
      
        
        public RestModule() : base("rest")
        {   
            Post["/updateVal"] = parameters =>
            {
                string str;
                using (var reader = new StreamReader(this.Request.Body))
                    str = reader.ReadToEnd();
                Console.WriteLine(str);
                UpdateValue upVal = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateValue>(str);
                
                Console.WriteLine("Got Post val " + upVal.value);

                ushort msgIdp;
                MqttClient cliento = new MqttClient("broker.hivemq.com");
                

                msgIdp = cliento.Publish("/8406AndroidVal", // topic
                                           Encoding.UTF8.GetBytes(upVal.value.ToString()), // message body
                                           MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                                           true); // retained


                return Nancy.HttpStatusCode.OK;
            };
            Get["/timeInterval"] = parameters =>
              {
                  ReportValue rVal = new ReportValue();
                  rVal.value=Convert.ToInt32(Program.timeInterval);
                  rVal.nodeSerial = "123";
                  rVal.thingName = "Android";
                  var json = Newtonsoft.Json.JsonConvert.SerializeObject(rVal);
                  return json;
              };
        }
    }
}
