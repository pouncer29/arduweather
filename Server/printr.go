package main
import (
	"encoding/json"
	"math/rand"
	"fmt"
	"log"
	"net"
	"strconv"
	"strings"
	"time"
)

/* Structs */
type entry struct {
	 Humidity float32 `json:"Humidity"`
	 Temp float32 `json:"Temp"`
	 WindSpeed float32 `json:"WindSpeed"`
	 Brightness float32 `json:Brightness`
}
/* End Structs*/

func random(min, max int) int {
	return rand.Intn(max-min) + min
}

func main() {
	log.Println("Begin UDP Server");
	PORT := ":20001"
	s, err := net.ResolveUDPAddr("udp4", PORT)
	if err != nil {
		log.Fatal(err)
		return
	}

	connection, err := net.ListenUDP("udp4", s)
	if err != nil {
		log.Println(err)
		return
	}

	defer connection.Close()
	buffer := make([]byte, 1024)
	rand.Seed(time.Now().Unix())

	for {
		n, addr, err := connection.ReadFromUDP(buffer)
		fromClient := buffer[0:n-1]

		if strings.TrimSpace(string(buffer[0:n])) == "STOP" {
			log.Println("Exiting UDP server!")
			return
		}

		var newEntry entry
		data := []byte(strconv.Itoa(random(1, 1001)))

		log.Printf("RAW: %s",string(fromClient));

		json.Unmarshal(fromClient, &newEntry)
		toDatabase(&newEntry)

		log.Printf("data: %s\n", string(data))
		_, err = connection.WriteToUDP(data, addr)
		if err != nil {
			log.Println(err)
			return
		}
	}
}

func toDatabase(e *entry) string{
	log.Println("Sending to DB ...")
	timestamp := time.Now().Unix()
	log.Printf("Time is: %v",timestamp);
	row := fmt.Sprintf("T:%f, H:%f, WS:%f, BR: %f, Time: %v",
		e.Temp, e.Humidity, e.WindSpeed, e.Brightness, timestamp)
	log.Printf("Received: %s\n",row);
/*
	database, err := sql.Open("sqlite3","../../Databases/weather_data.db")
	if err != nil {
		log.Fatal(err.Error())
	}

	insertStatement := `INSERT INTO LIVE_WEATHER_DATA(
						Timestamp,
						Temperature,
						Humidity,
						Wind_Speed,
						Brightness
						)
						Values (?,?,?,?,?);`

	statement, err:= database.Prepare(insertStatement)
*/

	log.Println(row)
/*
	if err != nil{
		log.Printf("Statement: %s - %s",insertStatement, "FAILED")
	}

	_, err = statement.Exec(timestamp,e.Temp,e.Humidity,e.WindSpeed,e.Brightness)

	if err != nil{
		log.Printf("******* SOMETHING WENT WRONG WITH EXEC *****")
		log.Printf(err.Error());
	}

	database.Close();
*/
	return  row
}

