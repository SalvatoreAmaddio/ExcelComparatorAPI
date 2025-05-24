# Stage 1 Build the application
FROM mcr.microsoft.comdotnetsdk8.0 AS build
WORKDIR src

# Copy and restore dependencies
COPY .csproj .
RUN dotnet restore

# Copy the full source and build the project
COPY . .
RUN dotnet publish -c Release -o apppublish

# Stage 2 Run the application
FROM mcr.microsoft.comdotnetaspnet8.0 AS final
WORKDIR app

COPY --from=build apppublish .

ENTRYPOINT [dotnet, ExcelComparatorAPI.dll]
