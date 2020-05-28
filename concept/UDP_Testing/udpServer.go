package main
import (
	"encoding/json"
	"fmt"
	"math/rand"
	"net"
	"strconv"
	"strings"
	"time"
)

type entry struct {
	 Time string `json:"Time"`
	 Humidity string`json:"Humidity"`
	 Temp string`json:"Temp"`
	 WindSpeed string`json:"WindSpeed"`
	 WindDir string `json:"WindDir"`
}

func random(min, max int) int {
	return rand.Intn(max-min) + min
}

func main() {
	fmt.Println("Begin UDP Server");
	PORT := ":20001"
	s, err := net.ResolveUDPAddr("udp4", PORT)
	if err != nil {
		fmt.Println(err)
		return
	}

	connection, err := net.ListenUDP("udp4", s)
	if err != nil {
		fmt.Println(err)
		return
	}

	defer connection.Close()
	buffer := make([]byte, 1024)
	rand.Seed(time.Now().Unix())

	for {
		n, addr, err := connection.ReadFromUDP(buffer)
		fromClient := buffer[0:n-1]
		//fmt.Print("-> ", fromClient)

		if strings.TrimSpace(string(buffer[0:n])) == "STOP" {
			fmt.Println("Exiting UDP server!")
			return
		}

		var newEntry entry
		json.Unmarshal(fromClient,&newEntry)
		toDatabase(&newEntry)

		data := []byte(strconv.Itoa(random(1, 1001)))
		fmt.Printf("data: %s\n", string(data))
		_, err = connection.WriteToUDP(data, addr)
		if err != nil {
			fmt.Println(err)
			return
		}
	}
}

func toDatabase(e *entry) string{
	println("Sending to DB ...")
	row := fmt.Sprintf("T:%s, H:%s, WS:%s, WD: %s, Time: %s",
		e.Temp, e.Humidity, e.WindSpeed, e.WindDir, e.Time)
	println(row)
	return  row
}

