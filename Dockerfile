#See https://aka.ms/containerfastmode to understand how VS uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["iCache.API/iCache.API.csproj", "iCache.API/"]
COPY ["iCache.Common/iCache.Common.csproj", "iCache.Common/"]
RUN dotnet restore "iCache.API/iCache.API.csproj"
COPY . .
WORKDIR "/src/iCache.API"
RUN dotnet build "iCache.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "iCache.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "iCache.API.dll"]