
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


int status = WL_IDLE_STATUS;
#include "arduino_secrets.h" 
///////please enter your sensitive data in the Secret tab/arduino_secrets.h
char ssid[] = SECRET_SSID;        // your network SSID (name)
char pass[] = SECRET_PASS;    // your network password (use for WPA, or use as key for WEP)
int keyIndex = 0;            // your network key Index number (needed only for WEP)

unsigned int localPort = 2390;      // local port to listen on

char packetBuffer[255]; //buffer to hold incoming packet
char  ReplyBuffer[] = "{\"Humidity\": \"36.43\", \"Temp\": \"56.24\", \"WindSpeed\": \"53.63\", \"Brightness\": \"80.74\"}";


WiFiUDP Udp;

void setup() {
  //Initialize serial and wait for port to open:
  Serial.begin(9600);
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
  // if you get a connection, report back via serial:
  Udp.begin(localPort);
}


float getTemperature(){
  return analogRead(A0);
}

float getHumidity(){
  return 6;
}

float getBright(){
  return analogRead(A1);
}

float getWindSpeed(){
  return 12;
}

/** Entry Creation */
StaticJsonDocument<256> createEntry(){
  StaticJsonDocument<256> entry;
  float temp = getTemperature();
  float humidity = getHumidity();
  float brightness = getBright();
  float windSpeed = getWindSpeed();
  
  entry["Temp"] = temp;
  entry["Humidity"] = humidity;
  entry["Brightness"] = brightness;
  entry["WindSpeed"] = windSpeed;

  return entry;
}

void loop() {

  Serial.println("Looping");
 
    IPAddress remoteIP(172,16,1,74);
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

    //uint64_t delayTime = 60UL * 60UL * 1000UL;
    delay(15000);
  
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
