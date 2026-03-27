# 階段 1：編譯環境 (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. 複製所有專案檔 (.csproj) 以進行 Restore
COPY ["SalterWebApi/SalterWebApi.csproj", "SalterWebApi/"]
COPY ["SalterEFModels/SalterEFModels.csproj", "SalterEFModels/"]
COPY ["ForumServiceHelper/ForumServiceHelper.csproj", "ForumServiceHelper/"]
COPY ["ForumRepositoryHelper/ForumRepositoryHelper.csproj", "ForumRepositoryHelper/"]
COPY ["ExpRepositoryHelper/ExpRepositoryHelper.csproj", "ExpRepositoryHelper/"]
COPY ["ExpServiceHelper/ExpServiceHelper.csproj", "ExpServiceHelper/"]
COPY ["Home/HomeRepositoryHelper.csproj", "Home/"]
COPY ["Home2/HomeServiceHelper.csproj", "Home2/"]
COPY ["TripRepositoryHelper/TripRepositoryHelper.csproj", "TripRepositoryHelper/"]
COPY ["TripServiceHelper/TripServiceHelper.csproj", "TripServiceHelper/"]
COPY ["UserRepositoryHelper/UserRepositoryHelper.csproj", "UserRepositoryHelper/"]
COPY ["UserServiceHelper/UserServiceHelper.csproj", "UserServiceHelper/"]

# 執行還原 (Restore)
RUN dotnet restore "SalterWebApi/SalterWebApi.csproj"

# 2. 複製剩餘的所有原始碼
COPY . .

# 3. 切換到 WebApi 目錄進行發行 (Publish)
WORKDIR "/src/SalterWebApi"
RUN dotnet publish "SalterWebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 階段 2：執行環境 (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 設定環境變數 (確保容器內監聽 8081)
ENV ASPNETCORE_URLS=http://+:8081

EXPOSE 8081
ENTRYPOINT ["dotnet", "SalterWebApi.dll"]