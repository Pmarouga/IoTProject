#include <ArduinoJson.h>
#include <Wifi.h>;

char rxBuffer[3];
unsigned long lastSer=0;
int state;
bool check1 = false, check2 = false, iread=false;

void handleData(char data[3]) {
	
	
	if (!(check1 && check2)) {
		if (data[0] == data[1] == data[2] == 0x01) {
			Serial.write(0x01);
			check1 = true;
		}
		else if (data[0] == data[1] == data[2] == 0x02 && check1) {
			Serial.write(0x02);
			check2 = true;
		}
		else {
			Serial.write(0x03);
			check1 = false;
			check2 = false;
		}
	}
	else {
		state = data[0];
		Serial.write(0x10);
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
	iread = false;
	//digitalWrite(LED_BUILTIN, LOW);
	while (Serial.available()>0) {
		digitalWrite(LED_BUILTIN, HIGH);
		*rxBuffer += Serial.read();
		lastSer = millis();
		iread = true;
	}
	if (millis() - lastSer > 5000 && iread) {
		handleData(rxBuffer);
		digitalWrite(LED_BUILTIN, HIGH);
	}
	
	
}
