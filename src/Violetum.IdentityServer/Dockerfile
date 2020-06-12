FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

WORKDIR /app
EXPOSE 5000

#RUN apt-get update && \
#    apt-get install -y openssl && \
#    openssl req -newkey rsa:2048 -new -nodes -x509 -days 3650 -keyout key.pem -out cert.pem \
#        -subj "/C=GE/ST=Tbilisi/L=Tbilisi/O=Joffarex/OU=IT Department/CN=joffarex@gmail.com" && \
#    openssl pkcs12 -export -in cert.pem -inkey key.pem -out cert.pfx -password pass:password
    
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