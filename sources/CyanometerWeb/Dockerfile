FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Cyanometer.Web/Cyanometer.Web.csproj", "CyanometerWeb/"]
COPY . .
RUN dotnet restore "CyanometerWeb/Cyanometer.Web.csproj"
WORKDIR "/src/CyanometerWeb"
RUN dotnet build "Cyanometer.Web.csproj" -c Release -o /app/build
WORKDIR "/src/Test/Cyanometer.WebTest"
RUN dotnet test "Cyanometer.WebTest.csproj" -c Release

WORKDIR "/src/CyanometerWeb"
FROM build AS publish
RUN dotnet publish "Cyanometer.Web.csproj" -c Release -o /app/publish

FROM base AS final
# install slovene language
RUN apt-get clean && apt-get -y update && apt-get install -y locales && locale-gen sl_SI.UTF-8 && locale -a \
	&& apt-get clean
# && localedef -i sl_SI -c -f UTF-8 -A /usr/share/locale/locale.alias sl_SI.UTF-8 && LANG sl_SI.utf8 
WORKDIR /app
COPY --from=publish /app/publish .
# use non-root user
USER 9000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Cyanometer.Web.dll"]