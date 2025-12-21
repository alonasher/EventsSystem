FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EventsApi.csproj", "./"]
RUN dotnet restore "EventsApi.csproj"
COPY . .
RUN dotnet publish "EventsApi.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EventsApi.dll"]