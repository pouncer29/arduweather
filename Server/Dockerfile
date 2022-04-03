FROM golang:1.18-alpine
WORKDIR /usr/src/app

# Copy app files
COPY go.mod go.sum ./
RUN go mod download && go mod verify
COPY ./udpServer.go ./udpServer.go
RUN go build -v -o /usr/local/bin/adw_receiver ./...

CMD ["adw_receiver"]
