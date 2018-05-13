#include <ArduinoJson.h>
#include <Wifi.h>;

byte rxBuffer[3];
int rxBufferIndex = 0;
unsigned long lastSer=0;
int state;
bool check1 = false, check2 = false;

void handleData(byte data[3]) {
	
	digitalWrite(LED_BUILTIN, HIGH);
	delay(1000);
	if (!(check1 && check2)) {
		if (data[0] == data[1] == data[2] == byte{ 0x01 }) {
			Serial.write(byte{ 0x01 });
			check1 = true;
		}
		else if (data[0] == data[1] == data[2] == byte{ 0x02 }&& check1) {
			Serial.write(byte{ 0x02 });
			check2 = true;
		}
		else {
			Serial.write(byte{ 0x03 });
			check1 = false;
			check2 = false;
		}
	}
	else {
		state = data[0];
		Serial.write(byte{ 0x10 });
		check1 = false;
		check2 = false;
	}

}


// the setup function runs once when you press reset or power the board
void setup() {
	pinMode(LED_BUILTIN, OUTPUT);
	Serial.begin(115200);
	digitalWrite(LED_BUILTIN, LOW);
}
// the loop function runs over and over again until power down or reset
void loop() {
	digitalWrite(LED_BUILTIN, LOW);
	//digitalWrite(LED_BUILTIN, LOW);
	while (Serial.available()>0) {
		rxBuffer[rxBufferIndex] +=(byte)Serial.read();
		rxBufferIndex++;
		lastSer = millis();
		//digitalWrite(LED_BUILTIN, HIGH);
	}
	if (millis() - lastSer > 5000) {
		handleData(rxBuffer);
		
		for (int j = 0; j < rxBufferIndex; j++) {
			rxBuffer[j] = 0;
			rxBufferIndex = 0;
		}
	}
	
	
}
