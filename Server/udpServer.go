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
	"context"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

/* Structs */
type entry struct {
	 Humidity float32
	 Temp float32
	 Brightness float32
	 WindSpeed float32
	 WindDir string
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

		log.Printf("data: %s\n", string(data))
		log.Printf("from_client: %s \n",fromClient)
 		err = json.Unmarshal([]byte(fromClient), &newEntry)
 		if err != nil{
 		    log.Fatal(err)
 		}
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
	log.Printf("Time: %v",timestamp)
	row := fmt.Sprintf("T:%f, H:%f, WS:%f, BR: %f, WD: %s Time: %i",
		e.Temp, e.Humidity, e.WindSpeed, e.Brightness, e.WindDir,timestamp)
	log.Printf("Received %s\n",row);

	client,err := mongo.NewClient(options.Client().ApplyURI("mongodb://127.0.0.1:27017/"))
	if err != nil{
		log.Fatal(err)
	}
	ctx, _ := context.WithTimeout(context.Background(),30*time.Second)
	err = client.Connect(ctx)
	if err != nil{
		log.Fatal(err)
	}
	defer client.Disconnect(ctx)

	//Grab Collection
	adwdb := client.Database("adwdb")
	liveWeatherCollection := adwdb.Collection("reports")

	entryResult,err:= liveWeatherCollection.InsertOne(ctx, bson.D{
		{Key:"timestamp",Value:timestamp},
		{Key:"temp",Value:e.Temp},
		{Key:"humidity",Value:e.Humidity},
		{Key:"brightness",Value:e.Brightness},
		{Key:"windSpeed",Value:e.WindSpeed},
		{Key:"windDir",Value:e.WindDir},
		})

	if(err != nil){
		log.Fatal(err)
	} else {
		log.Printf("Inserted %v: %s\n",entryResult.InsertedID,row);
	}

	return  row
}

