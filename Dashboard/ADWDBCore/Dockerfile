FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

#Set Timezone
ENV TZ="America/Regina"

# Copy sln
COPY *.sln .

#copy the dbman
COPY ./DBManager/ ./DBManager

#copy the Core
COPY ./ADWDBCore/ ./ADWDBCore

#RUN dotnet restore

WORKDIR /app
RUN dotnet publish -c Release -o out 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app 

#Run it
COPY --from=build /app/out ./
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "ADWDBCore.dll"]
