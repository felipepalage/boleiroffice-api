FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Boleiroffice.Api/Boleiroffice.Api.csproj", "Boleiroffice.Api/"]
COPY ["Boleiroffice.Application/Boleiroffice.Application.csproj", "Boleiroffice.Application/"]
COPY ["Boleiroffice.Domain/Boleiroffice.Domain.csproj", "Boleiroffice.Domain/"]
COPY ["Boleiroffice.Infrastructure/Boleiroffice.Infrastructure.csproj", "Boleiroffice.Infrastructure/"]
RUN dotnet restore "Boleiroffice.Api/Boleiroffice.Api.csproj"
COPY Boleiroffice.Api/ Boleiroffice.Api/
COPY Boleiroffice.Application/ Boleiroffice.Application/
COPY Boleiroffice.Domain/ Boleiroffice.Domain/
COPY Boleiroffice.Infrastructure/ Boleiroffice.Infrastructure/
COPY Directory.Packages.props .
RUN dotnet publish "Boleiroffice.Api/Boleiroffice.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Boleiroffice.Api.dll"]
