
/*
  WiFi UDP Send and Receive String

 This sketch wait an UDP packet on localPort using the WiFi module.
 When a packet is received an Acknowledge packet is sent to the client on port remotePort

 created 30 December 2012
 by dlf (Metodo2 srl)

 */


#include <SPI.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>
#include <ArduinoJson.h>
#include "DHT.h"

int status = WL_IDLE_STATUS;
#include "arduino_secrets.h" 
///////please enter your sensitive data in the Secret tab/arduino_secrets.h
char ssid[] = SECRET_SSID;        // your network SSID (name)
char pass[] = SECRET_PASS;    // your network password (use for WPA, or use as key for WEP)
int keyIndex = 0;            // your network key Index number (needed only for WEP)

//Sensors
#define DHTTYPE DHT11   // DHT 11
#define DHTPIN 2
DHT dht(DHTPIN, DHTTYPE);


//Timing
unsigned long prevMS_read;
unsigned long prevMS_send;
unsigned long sendTime;
unsigned long readWindSpeedTime = 10UL;
bool initialRun;
float maxWindReading;

unsigned int localPort = 2390;      // local port to listen on

char packetBuffer[255]; //buffer to hold incoming packet

WiFiUDP Udp;

void setup() {
  
  maxWindReading = 0;
  //sendTime = 60UL * 10000UL;
  sendTime = 30000UL;
  prevMS_send = 0;
  prevMS_read = 0;  
  //Initialize serial and wait for port to open:
  Serial.begin(9600);
  dht.begin();
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  // check for the WiFi module:
  if (WiFi.status() == WL_NO_MODULE) {
    Serial.println("Communication with WiFi module failed!");
    // don't continue
    while (true);
  }

  String fv = WiFi.firmwareVersion();
  if (fv < WIFI_FIRMWARE_LATEST_VERSION) {
    Serial.println("Please upgrade the firmware");
  }

  // attempt to connect to Wifi network:
  while (status != WL_CONNECTED) {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid, pass);

    // wait 10 seconds for connection:
    delay(10000);
  }
  Serial.println("Connected to wifi");
  printWifiStatus();

  Serial.println("\nStarting connection to server...");
  Serial.print("Send interval is:");Serial.print(sendTime);Serial.println(" ms");
  // if you get a connection, report back via serial:
  Udp.begin(localPort);
}


float getTemperature(){

  float tempTwo = dht.readTemperature();
  Serial.print("Temperature Readings were:");Serial.println(tempTwo);
//  float avgTemp = (temp + tempTwo)/2.0;
  return tempTwo;
}

float getHumidity(){
  return dht.readHumidity();;
}

float getBright(){
  return analogRead(A1);
}

void updateWindSpeed(){
  float reading = analogRead(A2);
  //Serial.print("************************Current ");Serial.print(maxWindReading);Serial.print("Reading: ");Serial.println(reading);
  if(reading > maxWindReading){
    //Serial.print("************************Replacing: ");Serial.print(maxWindReading);Serial.print("With: ");Serial.println(reading);
    maxWindReading = reading;
  }
}

float getWindSpeed(){
  return maxWindReading;
}

/** Entry Creation */
StaticJsonDocument<256> createEntry(){
  StaticJsonDocument<256> entry;
  float temp = getTemperature();
  float humidity = getHumidity();
  float brightness = 0.0; //getBright();
  float windSpeed = 0.0; //getWindSpeed();
  
  entry["Temp"] = temp;
  entry["Humidity"] = humidity;
  entry["Brightness"] = brightness;
  entry["WindSpeed"] = windSpeed;
  entry["WindDir"] = "N/A";
  
  return entry;
}

void loop() {
  
  //Serial.println("Looping");

  unsigned long currentMS = millis();
  unsigned long elapsed_readSpeed = currentMS - prevMS_read;
  unsigned long elapsed_sendData = currentMS - prevMS_send;
  
  if(elapsed_readSpeed > readWindSpeedTime){
    //Serial.println("updating");
    updateWindSpeed();
    prevMS_read = currentMS;
  } else if (elapsed_sendData > sendTime){
    //Serial.println("Sending details");
    
    //IPAddress remoteIP(SERVER_IP); //TODO: SWITCH THIS 
	IPAddress remoteIP;
	remoteIP.fromString(SERVER_IP);
    Serial.print(remoteIP);
    Serial.println(20001);

    StaticJsonDocument<256> doc = createEntry();

    // send a reply, to the IP address and port that sent us the packet we received
    Udp.beginPacket(remoteIP, 20001);
    Serial.println("Sending:");
    serializeJson(doc,Serial);
    serializeJson(doc,Udp);
    Udp.println();
    Udp.endPacket();
    prevMS_send = currentMS;
    maxWindReading  = 0;
  }
  if(initialRun){
    initialRun=false;
  }
  //Serial.print("Elapsed:");Serial.println(elapsed);
  
  
}


void printWifiStatus() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print your board's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
}
