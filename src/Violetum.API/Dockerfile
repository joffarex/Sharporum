FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src
COPY ["src/Violetum.API/Violetum.API.csproj", "src/Violetum.API/"]
COPY ["src/Violetum.Infrastructure/Violetum.Infrastructure.csproj", "src/Violetum.Infrastructure/"]
COPY ["src/Violetum.Domain/Violetum.Domain.csproj", "src/Violetum.Domain/"]
COPY ["src/Violetum.ApplicationCore/Violetum.ApplicationCore.csproj", "src/Violetum.ApplicationCore/"]
RUN dotnet restore "src/Violetum.API/Violetum.API.csproj"
COPY . .
WORKDIR "/src/src/Violetum.API"
RUN dotnet build "Violetum.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Violetum.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Violetum.API.dll"]
