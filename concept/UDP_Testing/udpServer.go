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
	 Time int64 `json:"time"`
	 Humidity float32 `json:"humidity"`
	 Temp float32 `json:"temp"`
	 WindSpeed float32 `json:"wind_speed"`
	 WindDir string `json:"wind_dir"`
}

func random(min, max int) int {
	return rand.Intn(max-min) + min
}

func main() {
	received:= entry{}
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
		fmt.Print("-> ", string(buffer[0:n-1]))

		if strings.TrimSpace(string(buffer[0:n])) == "STOP" {
			fmt.Println("Exiting UDP server!")
			return
		}

		data := []byte(strconv.Itoa(random(1, 1001)))

		json.Unmarshal([]byte(data),&received)
		toDatabase(&received)

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
	row := fmt.Sprintf("T:%f, H:%f, WS:%f, WD: %s, Time: %s",
		e.Temp, e.Humidity, e.WindSpeed, e.WindDir, time.Unix(e.Time,0))
	return  row
}

