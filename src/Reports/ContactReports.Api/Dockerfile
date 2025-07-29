FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Reports/ContactReports.Api

COPY src/Reports/ContactReports.Api/ContactReports.Api.csproj ./

COPY src/Reports/ContactReports.Application/ContactReports.Application.csproj ../ContactReports.Application/
COPY src/Reports/ContactReports.Domain/ContactReports.Domain.csproj ../ContactReports.Domain/
COPY src/Reports/ContactReports.Infrastructure/ContactReports.Infrastructure.csproj ../ContactReports.Infrastructure/
COPY src/Contacts/Contacts.Domain/Contacts.Domain.csproj ../../Contacts/Contacts.Domain/

RUN dotnet restore "ContactReports.Api.csproj"

COPY src/Reports/ContactReports.Api/. ./
COPY src/Reports/ContactReports.Application/. ../ContactReports.Application/
COPY src/Reports/ContactReports.Domain/. ../ContactReports.Domain/
COPY src/Reports/ContactReports.Infrastructure/. ../ContactReports.Infrastructure/
COPY src/Contacts/Contacts.Domain/. ../../Contacts/Contacts.Domain/

RUN dotnet build "ContactReports.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/Reports/ContactReports.Api
RUN dotnet publish "ContactReports.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactReports.Api.dll"]