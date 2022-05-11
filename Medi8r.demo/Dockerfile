FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Medi8r.demo/Medi8r.demo.csproj", "Medi8r.demo/"]
RUN dotnet restore "Medi8r.demo/Medi8r.demo.csproj"
COPY . .
WORKDIR "/src/Medi8r.demo"
RUN dotnet build "Medi8r.demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Medi8r.demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Medi8r.demo.dll"]
