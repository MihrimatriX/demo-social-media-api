# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["DemoSocialMedia.sln", "./"]
COPY ["DemoSocialMedia.Api/DemoSocialMedia.Api.csproj", "DemoSocialMedia.Api/"]
COPY ["DemoSocialMedia.Application/DemoSocialMedia.Application.csproj", "DemoSocialMedia.Application/"]
COPY ["DemoSocialMedia.Domain/DemoSocialMedia.Domain.csproj", "DemoSocialMedia.Domain/"]
COPY ["DemoSocialMedia.Infrastructure/DemoSocialMedia.Infrastructure.csproj", "DemoSocialMedia.Infrastructure/"]

RUN dotnet restore "DemoSocialMedia.Api/DemoSocialMedia.Api.csproj"

COPY . .
WORKDIR "/src/DemoSocialMedia.Api"
RUN dotnet publish "DemoSocialMedia.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DemoSocialMedia.Api.dll"]
