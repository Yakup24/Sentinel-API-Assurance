# Sentinel API Assurance Framework

GitHub repository name: `sentinel-api-assurance`

Kurumsal SOAP/API servis regresyon otomasyon framework'u.

Bu proje VOLTRAN servisleri icin test suite, servis registry, request template, guvenlik politikasi ve HTML/JSON/CSV raporlama saglar. Eski Autopilot `<call active="true" service="..." operation="..." />` formatini da okuyabilir.

## Kapsam

- 20 servis registry kaydi
- 148 SOAP operasyonu
- 147 aktif test case
- 148 request body template dosyasi
- 1 operasyon bilincli kapali: `VirtualMsisdn_v1.0.submitOrder`
- State-changing operasyonlar varsayilan olarak bloklu

## Proje yapisi

```text
src/SentinelApiAssurance/
|-- Catalog/
|   `-- voltran-service-catalog.json
|-- Configuration/
|-- Execution/
|-- Models/
|-- Reporting/
|-- Requests/
|-- Safety/
|-- Services/
|-- Suites/
|   |-- voltran-enterprise-regression-suite.json
|   `-- voltran-smoke-suite.json
|-- Utilities/
|-- appsettings.json
|-- test-calls.xml
|-- SentinelApiAssurance.csproj
`-- Program.cs
```

## Calistirma

```powershell
cd C:\Users\yakup\Downloads\EnterpriseSoapApiTestAutomation\EnterpriseSoapApiTestAutomation\src\SentinelApiAssurance
dotnet build SentinelApiAssurance.csproj
```

Gercek endpoint'e gitmeden suite kontrolu:

```powershell
dotnet run --project SentinelApiAssurance.csproj -- --dry-run
```

STB ortaminda varsayilan regression suite:

```powershell
dotnet run --project SentinelApiAssurance.csproj -- --env STB
```

PRP ortaminda ayni suite:

```powershell
dotnet run --project SentinelApiAssurance.csproj -- --env PRP --suite Suites/voltran-enterprise-regression-suite.json
```

Legacy call XML ile:

```powershell
dotnet run --project SentinelApiAssurance.csproj -- --env STB --calls test-calls.xml
```

## Konfigurasyon

`appsettings.json` icinde ortam URL'leri, servis endpoint'leri, retry/timeout degerleri ve test verileri yonetilir.

Template degiskenleri:

```xml
<msisdn>{{Msisdn}}</msisdn>
<customerId>{{CustomerId}}</customerId>
```

Environment variable okumak icin:

```xml
<token>{{ENV:VOLTRAN_TEST_TOKEN}}</token>
```

## Request template mantigi

`Requests/<Service>/<Operation>.xml` dosyalari otomatik uretildi. Bunlar WSDL olmadan hazirlanan doldurulabilir sablonlardir. Canli regresyon kosusu icin her operasyonun body alanlari ilgili WSDL kontratina gore netlestirilmelidir.

Ornek:

```xml
<ser:getAddressByMsisdn xmlns:ser="http://voltran.local/AddressOperations_v1.0">
  <request>
    <msisdn>{{Msisdn}}</msisdn>
  </request>
</ser:getAddressByMsisdn>
```

## Guvenlik politikasi

Asagidaki tarz operasyonlar varsayilan olarak `Skipped` olur:

- `create*`
- `submit*`
- `activate*`
- `deactivate*`
- `remove*`
- `cancel*`
- `upsert*`
- `update*`
- `insert*`
- `set*`
- `inform*`

Sadece onayli test datasiyla kosmak icin ilgili case icinde:

```json
"AllowStateChangingOperation": true
```

olarak acilabilir.

## Raporlar

Kosum sonunda `bin/Debug/net8.0/Reports` altina su raporlar yazilir:

- HTML summary
- JSON result
- CSV result

HTML raporda genel sonuc ve servis bazli ozet bulunur.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
