FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Contacts/Contacts.Api

COPY src/Contacts/Contacts.Api/Contacts.Api.csproj ./
COPY src/Contacts/Contacts.Application/Contacts.Application.csproj ../Contacts.Application/
COPY src/Contacts/Contacts.Domain/Contacts.Domain.csproj ../Contacts.Domain/
COPY src/Contacts/Contacts.Infrastructure/Contacts.Infrastructure.csproj ../Contacts.Infrastructure/

RUN dotnet restore "Contacts.Api.csproj"

COPY src/Contacts/Contacts.Api/. ./
COPY src/Contacts/Contacts.Application/. ../Contacts.Application/
COPY src/Contacts/Contacts.Domain/. ../Contacts.Domain/
COPY src/Contacts/Contacts.Infrastructure/. ../Contacts.Infrastructure/

RUN dotnet build "Contacts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Contacts/Contacts.Api
RUN dotnet publish "Contacts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contacts.Api.dll"]