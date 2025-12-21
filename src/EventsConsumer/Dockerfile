FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EventsConsumer.csproj", "./"]
RUN dotnet restore "EventsConsumer.csproj"
COPY . .
RUN dotnet publish "EventsConsumer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EventsConsumer.dll"]