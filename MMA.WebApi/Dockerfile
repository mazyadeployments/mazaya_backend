#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["SMA.WebApi/SMA.WebApi.csproj", "SMA.WebApi/"]
RUN dotnet restore "SMA.WebApi/SMA.WebApi.csproj"
COPY . .
WORKDIR "/src/SMA.WebApi"
RUN dotnet build "SMA.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SMA.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SMA.WebApi.dll"]