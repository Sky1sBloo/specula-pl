FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY parser-run/*.csproj ./parser-run/
RUN cd parser-run && dotnet restore
COPY parser-run/ ./parser-run/
RUN cd parser-run && dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
CMD ["dotnet", "SpeculaSyntaxAnalyzer.dll"]
