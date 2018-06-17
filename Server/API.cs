using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class IoTNode
    {
        public string name;
        public string serialNumber;
        public string manufacturer;
        public string firmware;
        public List<IoTThing> things;
        public string apiVersion;

    }

    public enum SensorType
    {
        Temperature,
        Light,
        Humidity
    }
    public class IoTThing
    {
        public string name;
        public string manufacturer;
        public SensorType sensorType;
        public int state;
        public string serialNode;

    }

    public class Registration
    {
        public IoTNode nodeDescriptor;
    }

    public class GenericStateMsg
    {
        public string thingName;
        public string nodeSerial;
        public int value;
    }

    public class ReportValue : GenericStateMsg { }

    public class UpdateValue : GenericStateMsg { }

    public class APIMsg
    {
        public object msg;
        public MsgType msgType;
    }

    public enum MsgType
    {
        None,
        RegistrationThing,
        RegistrationNode,
        UpdateValue
    }
}