FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FGC.Payments.Api/FGC.Payments.Api.csproj", "FGC.Payments.Api/"]
COPY ["FGC.Payments.Application/FGC.Payments.Application.csproj", "FGC.Payments.Application/"]
COPY ["FGC.Payments.Infrastructure/FGC.Payments.Infrastructure.csproj", "FGC.Payments.Infrastructure/"]
COPY ["FGC.Payments.Domain/FGC.Payments.Domain.csproj", "FGC.Payments.Domain/"]
RUN dotnet restore "FGC.Payments.Api/FGC.Payments.Api.csproj"

COPY . .
WORKDIR "/src/FGC.Payments.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FGC.Payments.Api.dll"]
