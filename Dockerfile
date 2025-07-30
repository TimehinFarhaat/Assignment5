# Use official ASP.NET runtime image as base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy .csproj and restore
COPY ["CSharpMvcBasics.csproj", "./"]
RUN dotnet restore "./CSharpMvcBasics.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "CSharpMvcBasics.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CSharpMvcBasics.dll"]
