FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
#COPY ["Cyanometer.Web/Cyanometer.Web.csproj", "CyanometerWeb/"]
COPY . .
RUN ls 
RUN dotnet restore "Cyanometer.Web/Cyanometer.Web.csproj"
WORKDIR "/src/Cyanometer.Web"
RUN dotnet build "Cyanometer.Web.csproj" -c Release -o /app/build
WORKDIR "/src/Test/Cyanometer.WebTest"
RUN dotnet test "Cyanometer.WebTest.csproj" -c Release

WORKDIR "/src/Cyanometer.Web"
FROM build AS publish
RUN dotnet publish "Cyanometer.Web.csproj" -c Release -o /app/publish

FROM base AS final
# install slovene language
# won't work on Alpine
RUN apt clean && apt -y update && apt install -y locales && locale-gen de_DE.UTF-8 && locale -a \
	&& apt clean
#    && localedef -i sl_SI -c -f UTF-8 -A /usr/share/locale/locale.alias sl_SI.UTF-8 && LANG sl_SI.utf8 
WORKDIR /app
COPY --from=publish /app/publish .
# use non-root user
USER 9000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Cyanometer.Web.dll"]