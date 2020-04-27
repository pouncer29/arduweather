package main

import "net"
import "fmt"
import "bufio"
import "time"
import "encoding/json"
// only needed below for sample processing

type entry struct {
	 Time int64 `json:"time"`
	 Humidity float32 `json:"humidity"`
	 Temp float32 `json:"temp"`
	 WindSpeed float32 `json:"wind_speed"`
	 WindDir string `json:"wind_dir"`
}

func main() {

	fmt.Println("Launching server...")

	// listen on all interfaces
	ln, _ := net.Listen("tcp", "127.0.0.1:8081")

	// accept connection on port
	conn, _ := ln.Accept()

	// run loop forever (or until ctrl-c)
	endComms := "no"
	for {
		if endComms == "no" {
			t2:= entry{}
			// will listen for message to process ending in newline (\n)
			message, _ := bufio.NewReader(conn).ReadString('}')
			//bmessage, _ := bufio.NewReader(conn).ReadBytes('}')
			received := string(message)
			//fmt.Printf("Received: %s\n", received)

			if received == "gb}"{
					endComms = "yes"
				continue
			}

			json.Unmarshal([]byte(received),&t2)
			// sample process for string received
			println("Received entry..." )
			fmt.Printf("logging: %s", toDatabase(&t2))

			conn.Write([]byte("logged to db!"))
		}else {
			conn.Write([]byte("end"))
			conn.Close()
			break
		}
	}
	return
}

func toDatabase(e *entry) string{
	println("Sending to DB ...")
	row := fmt.Sprintf("T:%f, H:%f, WS:%f, WD: %s, Time: %s",
		e.Temp, e.Humidity, e.WindSpeed, e.WindDir, time.Unix(e.Time,0))
	return  row
}
