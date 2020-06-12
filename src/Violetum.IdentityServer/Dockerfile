FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base

WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src

COPY ["src/Violetum.IdentityServer/Violetum.IdentityServer.csproj", "src/Violetum.IdentityServer/"]
COPY ["src/Violetum.Infrastructure/Violetum.Infrastructure.csproj", "src/Violetum.Infrastructure/"]
COPY ["src/Violetum.Domain/Violetum.Domain.csproj", "src/Violetum.Domain/"]
RUN dotnet restore "src/Violetum.IdentityServer/Violetum.IdentityServer.csproj"
COPY . .
WORKDIR "/src/src/Violetum.IdentityServer"
RUN dotnet build "Violetum.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Violetum.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Violetum.IdentityServer.dll"]