#include <ArduinoJson.h>
#include <Wifi.h>;
#include <HTTPClient.h>;

HardwareSerial Serial2(1);
#define SERIAL1_RXPIN 16
#define SERIAL1_TXPIN 17

char rxBuffer[4], timeInt;
int rxBufferIndex = 0;
unsigned long lastSer=0, lastTimeInterval=0;
int state, timeInterval=500;
bool check1 = false, check2 = false, newItem = false, iRead = false;

const char* ssid = "YOURSSID";
const char* password = "YOURPASSWORD";



union ArrayToInteger {
	byte array[4];
	uint32_t integer;
};

void handleData(char data[4]) {
	
	if (!(check1 && check2)) {
		if (data[0] ==  7) {
			if (data[1] == 7) {
				if (data[2] == 7) {
					Serial.write(char{ 0x07 });
					check1 = true;
				}
			}
		}
		else if (data[0] == 2) {
			if (data[1] == 2) {
				if (data[2] == 2) {
					while (timeInterval > millis() - lastTimeInterval);
					Serial.write(char{ 0x02 });
					check2 = true;
				}
			}
		}
		else {
			Serial.write(char(0x03));
			check1 = false;
			check2 = false;
		}
	}
	else 
	{
	
		ArrayToInteger converter = {data[0],data[1],data[2],data[3]};
		state = converter.integer;
		Serial.write(char{0x09});
		check1 = false;
		check2 = false;
		newItem = true;
	}

}

// the setup function runs once when you press reset or power the board
void setup() {
	pinMode(LED_BUILTIN, OUTPUT);
	Serial.begin(115200);
	Serial2.begin(115200);
	digitalWrite(LED_BUILTIN, LOW);
	WiFiClient wifi;

	check1 = check2 = false;

	delay(4000);  

	scanNetworks();
	WiFiReset();
	
	 while (WiFi.status() != WL_CONNECTED) {

		delay(5000);
		Serial.println("Connecting to WiFi..");
		Serial.println(WiFi.status());
		
		WiFi.begin(ssid, password);
		Serial.println(WiFi.status());
	}
	Serial.println("Connected to the WiFi network");
	
}
// the loop function runs over and over again until power down or reset
void loop() {
	digitalWrite(LED_BUILTIN, LOW);


	while (Serial.available()>0) {
		Serial.readBytes(&rxBuffer[rxBufferIndex],1);
		iRead = true;
		rxBufferIndex++;
		lastSer = millis();
		
	}
	if (millis() - lastSer > 5000 && iRead) {
		handleData(rxBuffer);
		lastSer = millis();
		

		rxBufferIndex = 0;
		iRead = false;
	}


	
	if (newItem) {

		if (WiFi.status() == WL_CONNECTED) {   
		
			HTTPClient http;
			http.begin("http://192.168.2.2:80/rest/updateVal");  
			http.addHeader("Content-Type", "application/json");  
			http.setAuthorization("lVDd51borSM66TdS5Ucr1rGNEAveCP");

			DynamicJsonBuffer jsonBuffer;
			String input =
				"{\"nodeSerial\":\"123456789\",\"thingName\":\"ESP32\",\"value\":1}";
			JsonObject& root = jsonBuffer.parseObject(input);
			root[String("value")] = state;

			
			root.printTo(input);
		
			int httpResponseCode = http.POST(input);  
			
					

			if (httpResponseCode>0) {
				Serial.write(httpResponseCode);
				String response = http.getString();                       

			}
			else {
				
				Serial.write(httpResponseCode);
				Serial.print("Error on sending POST: ");
				Serial.println(httpResponseCode);

			}

			

			http.end();  //Free resources
			lastTimeInterval = millis();

		}
		else {
			digitalWrite(LED_BUILTIN, HIGH);
			delay(1000);
		}
		newItem = false;
	}
	if (WiFi.status() == WL_CONNECTED) {
	
		DynamicJsonBuffer jsonBuffer;
		
		HTTPClient http;
		http.begin("http://192.168.2.2:80/rest/timeInterval");
		http.GET();
		
		timeInt=http.getStream();
		JsonObject& root = jsonBuffer.parseObject(timeInt);
		timeInterval = root[String("value")];
	}
}
void scanNetworks() {
	// scan for nearby networks:
	Serial.println("** Scan Networks **");
	byte numSsid = WiFi.scanNetworks();
	// print the list of networks seen:
	Serial.print("SSID List:");
	Serial.println(numSsid);
	// print the network number and name for each network found:
	for (int thisNet = 0; thisNet<numSsid; thisNet++) {
		Serial.print(thisNet);
		Serial.print(") Network: ");
		Serial.println(WiFi.SSID(thisNet));
	}
}

void WiFiReset() {
	WiFi.persistent(false);
	WiFi.disconnect();
	WiFi.mode(WIFI_OFF);
	WiFi.mode(WIFI_STA);
}
