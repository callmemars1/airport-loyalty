FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Airport.Backend/Airport.Backend.csproj", "Airport.Backend/"]
RUN dotnet restore "Airport.Backend/Airport.Backend.csproj"
COPY . .
WORKDIR "/src/Airport.Backend"
RUN dotnet build "Airport.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Airport.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Airport.Backend.dll"]
