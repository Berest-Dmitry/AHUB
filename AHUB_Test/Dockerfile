#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AHUB_Test/AHUB_Test.csproj", "AHUB_Test/"]
RUN dotnet restore "AHUB_Test/AHUB_Test.csproj"
COPY . .
WORKDIR "/src/AHUB_Test"
RUN dotnet build "AHUB_Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AHUB_Test.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AHUB_Test.dll"]