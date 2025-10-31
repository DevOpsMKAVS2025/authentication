FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

RUN dotnet tool install --global dotnet-ef

COPY . ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

RUN apt-get update && \
    apt-get install -y tzdata && \
    ln -fs /usr/share/zoneinfo/Europe/Belgrade /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata

ENV TZ=Europe/Belgrade
ENV DOTNET_RUNNING_IN_DOCKER=true

COPY --from=build /out .

RUN mkdir -p /app/../../../Logs

EXPOSE 5156

ENTRYPOINT ["dotnet", "Auth.dll"]