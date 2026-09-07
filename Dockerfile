# Yayınlanmış çıktıyı kopyalayan ince runtime imajı. Publish adımı workflow'da
# ayrı çalışıyor; böylece hata mesajları "docker build" gürültüsüne karışmıyor.
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY publish/ .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CiPilot.Api.dll"]
