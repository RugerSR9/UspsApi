#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["UspsApi.gRPC/UspsApi.gRPC.csproj", "UspsApi.gRPC/"]
RUN dotnet restore "UspsApi.gRPC/UspsApi.gRPC.csproj"
COPY . .
WORKDIR "/src/UspsApi.gRPC"
RUN dotnet build "UspsApi.gRPC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UspsApi.gRPC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UspsApi.gRPC.dll"]