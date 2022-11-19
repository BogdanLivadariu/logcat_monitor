FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["logcat_monitor.csproj", "."]
RUN dotnet restore

COPY . .

RUN dotnet build "logcat_monitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish \
	-c Release \
	-o /app/publish \
	/p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS final

WORKDIR /app
ENV ANDROID_TOOLS_VERSION=31.0.3p1-r8

RUN apk update \
	&& apk add --no-cache android-tools=${ANDROID_TOOLS_VERSION} \
	&& rm -rf /var/cache/*

COPY --from=publish /app/publish .

CMD ["dotnet", "logcat_monitor.dll"]
