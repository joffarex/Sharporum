FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base

WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src
COPY ["src/Sharporum.API/Sharporum.API.csproj", "src/Sharporum.API/"]
COPY ["src/Sharporum.Infrastructure/Sharporum.Infrastructure.csproj", "src/Sharporum.Infrastructure/"]
COPY ["src/Sharporum.Domain/Sharporum.Domain.csproj", "src/Sharporum.Domain/"]
COPY ["src/Sharporum.ApplicationCore/Sharporum.ApplicationCore.csproj", "src/Sharporum.ApplicationCore/"]
RUN dotnet restore "src/Sharporum.API/Sharporum.API.csproj"
COPY . .
WORKDIR "/src/src/Sharporum.API"
RUN dotnet build "Sharporum.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sharporum.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sharporum.API.dll"]
