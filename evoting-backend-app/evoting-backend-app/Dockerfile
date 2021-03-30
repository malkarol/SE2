#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src  # Change container current dir
COPY ["evoting-backend-app/evoting-backend-app.csproj", "evoting-backend-app/"]
RUN dotnet restore "evoting-backend-app/evoting-backend-app.csproj" # Restores the dependencies and tools of a project
COPY . . # Copy all files from local current dir to container current dir
WORKDIR "/src/evoting-backend-app" # Change local current dir
RUN dotnet build "evoting-backend-app.csproj" -c Release -o /app/build # Build project

FROM build AS publish
RUN dotnet publish "evoting-backend-app.csproj" -c Release -o /app/publish # Pack the application and its dependencies into a folder for deployment

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "evoting-backend-app.dll"] # Run