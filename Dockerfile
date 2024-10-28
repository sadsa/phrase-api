FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["PhraseApi.Api/*.csproj", "PhraseApi.Api/"]
COPY ["PhraseApi.Core/*.csproj", "PhraseApi.Core/"]
COPY ["PhraseApi.Infrastructure/*.csproj", "PhraseApi.Infrastructure/"]
RUN dotnet restore "PhraseApi.Api/PhraseApi.Api.csproj"

# Copy the rest of the code
COPY . .
RUN dotnet build "PhraseApi.Api/PhraseApi.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhraseApi.Api/PhraseApi.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhraseApi.Api.dll"]