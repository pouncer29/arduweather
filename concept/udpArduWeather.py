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

dbAddr = "gurgi"
dbPort = 20001

def generateRandomEntry():
	entry = dict()
	entry['Humidity'] = ("%.2f" % (random.random() * 100))
	entry['Temp'] = ("%.2f" % (random.random() * 100))
	entry['WindSpeed'] = ("%.2f" % (random.random() * 100))
	entry['Brightness'] = ("%.2f" % (random.random() * 100))
	
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

