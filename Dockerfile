#Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy only the necessary project files
COPY HorsesForCourses.Core/HorsesForCourses.Core.csproj ./HorsesForCourses.Core/
COPY HorsesForCourses.Service/HorsesForCourses.Service.csproj ./HorsesForCourses.Service/
COPY HorsesForCourses.MVC/HorsesForCourses.MVC.csproj ./HorsesForCourses.MVC/

# Restore dependencies for the MVC project only
RUN dotnet restore ./HorsesForCourses.MVC/HorsesForCourses.MVC.csproj

# Copy the rest of the source code
COPY . .

# Publish the MVC project
RUN dotnet publish ./HorsesForCourses.MVC/HorsesForCourses.MVC.csproj -c Release -o /app/publish

#Stage 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80 and set environment variable for URLs
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "HorsesForCourses.MVC.dll"]
