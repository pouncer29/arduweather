#!/usr/bin/env python3
#       Name: Frank Lewis
#       NSID: fbl773
# Student ID: 11194945
#    Lecture: 04
#   Tutorial: T08
# Assignment: lab _
#   Synopsis: UDP client for study!


import socket
import json
import time
import random

dbAddr = "172.16.1.69"
dbPort = 20001

def generateRandomEntry():
	entry = dict()
	entry['time'] = int(time.time())
	entry['humidity'] = (random.random() * 100)
	entry['temp'] = (random.random() * 100)
	entry['wind_speed'] =(random.random() * 100)
	entry['wind_dir'] = random.choice('nesw')
	
	return json.dumps(entry)

import socket

UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

while ...:

	toSend = generateRandomEntry()
	print("sending: " + toSend)

	bytesToSend         = str.encode(toSend + "}")
	serverAddressPort   = (dbAddr, dbPort)
	bufferSize          = 1024

	# Send to server using created UDP socket

	UDPClientSocket.sendto(bytesToSend, serverAddressPort)

	msgFromServer = UDPClientSocket.recvfrom(bufferSize)

	msg = "Message from Server {}".format(msgFromServer[0].decode('utf-8'))
	print(msg)

	time.sleep(10)

