# Sentinel API Assurance Framework

![.NET CI](https://github.com/Yakup24/Sentinel-API-Assurance/actions/workflows/dotnet-ci.yml/badge.svg)

**Sentinel API Assurance**, SOAP ve REST tabanlı kurumsal servislerin regresyon, smoke ve güvenlik kontrollü doğrulama süreçleri için geliştirilmiş .NET 8 tabanlı test otomasyon framework'üdür.

Projenin ana hedefi; servis operasyonlarını tek tek manuel denemek yerine, tanımlı suite dosyaları üzerinden otomatik çalıştırmak, riskli operasyonları güvenli şekilde bloklamak ve koşum sonucunu HTML/JSON/CSV raporlarıyla görünür hale getirmektir.

## Öne çıkan yetenekler

- STB / PRP gibi çoklu ortam yönetimi
- JSON tabanlı test suite yapısı
- Legacy Autopilot `<call active="true" service="..." operation="..." />` formatı desteği
- SOAP 1.1 / SOAP 1.2 request envelope üretimi
- REST test executor iskeleti
- Template değişkenleri: `{{Msisdn}}`, `{{CustomerId}}`, `{{ENV:TOKEN_NAME}}`
- Assertion engine:
  - HTTP status kontrolü
  - SOAP Fault kontrolü
  - Response contains / not contains
  - XML element exists
  - XML element equals
  - Maksimum response time kontrolü
- Riskli operasyon güvenlik politikası
- Retry / timeout yönetimi
- Dry-run kontrol modu
- HTML, JSON ve CSV raporlama
- GitHub Actions CI pipeline

## Mevcut kapsam

- 20 servis registry kaydı
- 148 SOAP operasyonu
- 147 aktif test case
- 148 request body template dosyası
- 1 operasyon bilinçli kapalı: `VirtualMsisdn_v1.0.submitOrder`
- State-changing operasyonlar varsayılan olarak bloklu

## Proje yapısı

```text
src/SentinelApiAssurance/
├─ Catalog/
│  └─ voltran-service-catalog.json
├─ Configuration/
├─ Execution/
├─ Models/
├─ Reporting/
├─ Requests/
├─ Safety/
├─ Services/
├─ Suites/
│  ├─ voltran-enterprise-regression-suite.json
│  └─ voltran-smoke-suite.json
├─ Utilities/
├─ appsettings.json
├─ test-calls.xml
├─ SentinelApiAssurance.csproj
└─ Program.cs
```

## Hızlı başlangıç

```powershell
git clone https://github.com/Yakup24/Sentinel-API-Assurance.git
cd Sentinel-API-Assurance

dotnet restore src/SentinelApiAssurance/SentinelApiAssurance.csproj
dotnet build src/SentinelApiAssurance/SentinelApiAssurance.csproj
```

Gerçek endpoint'e istek atmadan suite ve dosya kontrolü yapmak için:

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run
```

STB ortamında varsayılan regression suite'i çalıştırmak için:

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB
```

PRP ortamında belirli suite ile çalıştırmak için:

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env PRP --suite Suites/voltran-enterprise-regression-suite.json
```

Legacy call XML ile çalıştırmak için:

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --calls test-calls.xml
```

## Konfigürasyon

Ana ayarlar `src/SentinelApiAssurance/appsettings.json` içindedir.

```json
{
  "DefaultEnvironment": "STB",
  "DefaultSuitePath": "Suites/voltran-enterprise-regression-suite.json",
  "TimeoutSeconds": 30,
  "RetryCount": 1,
  "BlockDangerousOperationsWithoutExplicitApproval": true
}
```

Template değişkenleri request body ve header alanlarında kullanılabilir:

```xml
<msisdn>{{Msisdn}}</msisdn>
<customerId>{{CustomerId}}</customerId>
<token>{{ENV:VOLTRAN_TEST_TOKEN}}</token>
```

## Güvenlik politikası

Aşağıdaki türde operasyonlar varsayılan olarak `Skipped` olur:

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

State-changing operasyonu yalnızca onaylı test datası ile çalıştırmak için ilgili case içinde açık izin verilmelidir:

```json
"AllowStateChangingOperation": true
```

Ayrıntılı açıklama için: [`docs/OPERATION_SAFETY.md`](docs/OPERATION_SAFETY.md)

## Raporlar

Koşum sonunda raporlar uygulama çıktı klasöründeki `Reports` dizinine yazılır:

- HTML summary
- JSON result
- CSV result

HTML raporda genel özet, servis bazlı durum ve test case detayları bulunur.

## Dokümantasyon

- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)
- [`docs/OPERATION_SAFETY.md`](docs/OPERATION_SAFETY.md)
- [`docs/RUNBOOK.md`](docs/RUNBOOK.md)
- [`docs/TEST_STRATEGY.md`](docs/TEST_STRATEGY.md)

## Yol haritası

- WSDL parser ile operasyon keşfi
- Data-driven test desteği
- JUnit XML rapor çıktısı
- DB assertion modülü
- Masked response logging
- GitHub Actions artifact upload
- Basit web dashboard

## Lisans

This project is licensed under the MIT License. See [LICENSE](LICENSE).
