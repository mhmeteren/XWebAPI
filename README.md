[![build and test](https://github.com/mhmeteren/XWebAPI/actions/workflows/unit-tests.yml/badge.svg)](https://github.com/mhmeteren/XWebAPI/actions/workflows/unit-tests.yml)

<h1> Sosyal Medya Rest API Projesi </h1>

Monolitik yapıda, .Net Core 8 ile geliştirdiğim twitter (x) clone projesi. 
Projeyi yapmamdaki ana neden dotnet ile katmanlı mimari öğrenmekti. Bunu dışında bir API de olması gereken diğer özelikleride (Rate Limit, Error Handiling, Versioning vs.) öğrenmiş oldum. Bu öğrendiklerimi bölümler halinde kısaca açıklayacağım.


<h2> Bölümler </h2>

- [API Mimarisi](#api-mimarisi)
- [Database Diyagramı](#database-diyagramı)
- [Postman](#postman)
- [API](#api)
  - [Pagination](#pagination)
  - [Versioning](#versioning)
  - [Logging](#logging)
  - [Erreor Handiling](#erreor-handiling)
  - [Rate Limit](#rate-limit)
  - [Health Check](#health-check)
  - [Data Validation (Fluent Validation)](#data-validation-fluent-validation)
  - [Unit Test (XUnit)](#unit-test-xunit)
- [GitHub Actions](#github-actions)
- [File Storage](#file-storage)


## API Mimarisi
![System Design](/github/SystemDesign.jpg)


## Database Diyagramı
![Database Diagram](/github/DatabaseDiagram.jpg)



## Postman
[API Workspace](https://www.postman.com/systemofadown/workspace/xwebapi/overview)


## API
### Pagination
Her sorguda tüm verileri göndermek yerine bu verileri sayfalandırarak kullanıcıya sunmaktır. Bunu projede şu şekilde yapıyoruz:
**List** sınıfını generic olarak miras alan [PagedList](/Entities/RequestFeatures/PagedList.cs) sınıfını oluşturuyoruz. Bu sınıf gelen veri listesini [RequestParameters](/Entities/RequestFeatures/RequestParameters.cs) türünde gelen parametreler ile sayfalandırıyor ve toplam verinin ne kadar olduğu, kaç sayfa olduğu gibi verileri hesaplayıp [MetaData](/Entities/RequestFeatures/MetaData.cs) sınıfından sahip olduğu **MetaData** değikenine atıyor. Artık gelen her istekte gelen veri listesi bu şekilde sayfalanmış oluyor (Örn:  [GetAllFollowersAsync](/Repositories/EFCore/FollowsRepository.cs#L20)).
  
### Versioning
API'nin versiyonlaması, adından da anlaşılacağı gibi API'ı versiyonluyoruz. Bir endpoint'in geliştirilmesi, yeni özellikler eklenmesi gibi durumlarda versiyonlama kullanılır. Versiyonlama şu şekildedir:
- Öncelikle bağımlılıkları yüklüyoruz ([Asp.Versioning.Mvc](https://www.nuget.org/packages/Asp.Versioning.Mvc) ve [Asp.Versioning.Mvc.ApiExplorer](https://www.nuget.org/packages/Asp.Versioning.Mvc.ApiExplorer)).
- Program.cs de gerekli konfigürasyonları yapıyoruz. Bunu doğrudan Program.cs' e yazmadım, onun yerine extension olarak [ServicesExtensions](XWebAPI/Extensions/ServicesExtensions.cs#L63) 'e ekledim. Burada kısaca varsayılan sürümün ne oalcağı, sürüm belirlenemse ne yapılacağı, sürümün hangi formata ve nerelerden (Header, Url, Query) alınacağı gibi ayarlamaları yaptık.
- Controllerl'lardaki örnek kullanım: [TweetController](/Presentation/Controllers/v1/TweetController.cs#L14). API Url: `BaseUrl/api/v1/Tweet` veya `BaseUrl/api/v2/Tweet` gibi dir.


### Logging
Yazılım çalışırken oluşan durumları kaydederiz ve buna loglama diyoruz. Bu durumlar hatalar, uyarılar, bilgi mesajları vs. olabilir. Örneğin uygulamada oluşan bir hatayı loglama sayesinde daha hızlı tespit edip hatanın nereden ve ne zaman kaynaklandığını bulup ona göre bir reaksiyon alabiliriz. Bunu dışında logları uygulama dışında depolamak, analiz etmek veya sorgulamak için analiz motorları kullanılabilir. Buna örnek olarak [Elasticsearch](https://www.elastic.co) verilebilir. Bu projede de bunu kullandım. Logları hem API üzerinde ilgili [dosyada](XWebAPI/Logs/) tutuyorum hem de Elasticsearch' e göderiyorum. Loglama için [LoggerService](/Services/SerilogLoggerManager.cs) ve Serilog' un konfigürasyonu [appsettings.json](XWebAPI/appsettings.json#L2) da mevcutur.


### Erreor Handiling
Uygulamada çıkan bir exception' nın nasıl ele alınacağını belirleriz. Temel olarak bu işlem şu şekildedir:
- Projedeki [ExceptionMiddlewareExtensions](XWebAPI/Extensions/ExceptionMiddlewareExtensions.cs) yer aldığı gibi `UseExceptionHandler` ile istisnaları yakalar ve işleyiciyi çalıştırırız.
- Oluşan Hatanın özeliklerini alırız ([IExceptionHandlerFeature](/XWebAPI/Extensions/ExceptionMiddlewareExtensions.cs#L17)).
- Hatanın custom olarak oluşturduğumuz [`Exception` class'lardan](/Entities/Exceptions/) mı? yoksa farklı bir durumdan mı? olduğunu kontrol eder ona göre StatusCode ayarlarız ve yanıtı göndeririz. Burada sadece hatayı işleyip response dönülmez aynı zamanda bu hata [loglarınır](/XWebAPI/Extensions/ExceptionMiddlewareExtensions.cs#L29).
- Son olarak [Program.cs](/XWebAPI/Program.cs#L81)' de bu middleware çağırıyoruz.


### Rate Limit
Servislerin belirli bir zaman diliminde alabileceği isteklerin sayısını sınırlamaktır. Bu kısıtlama aşırı yüklenmeyi önlemek, hizmet kalitesini korumak ve güvenlik açıklarını engellemek için kullanılır. .Net Core' da kullanmak için:
- [AspNetCoreRateLimit](https://www.nuget.org/packages/AspNetCoreRateLimit) bağımlılığını yüklüyoruz.
- [appsetings.json](/XWebAPI/appsettings.json#L61)'a ilgili konfigürasyonu ekliyoruz (bunu bu şekilde yapmak yerine ilgili ayarları doğrudan statik olarak da yazabilirsiniz).
- [ServicesExtensions](XWebAPI/Extensions/ServicesExtensions.cs#L112) de bu konfigürasyonları ve diğer servislerin ayarlamalarını yapıyoruz.
- Bundan sonra tek yapmamız gereken bu methodu [Program.cs](/XWebAPI/Program.cs#L67)'de çağırmak ve [Program.cs](/XWebAPI/Program.cs#L99)' de Ip tabanlı rate limiting'i etkinleştirmek.    
- Artık API' ye rate limiting özelliği eklenmiş olacaktır. Örneğin benim [konfigürasyonuma](/XWebAPI/appsettings.json#L61) göre her client saniyede en fazla 5 istek atabilir eğer bu sınırı aşılırsa HTTP codu 429 olan bir hata alır.

### Health Check
Bir uygulamanın veya servisin sağlığını izlemek ve durumunu belirtmek için yapılan düzenli kontrollerdir. Bu Projede API' nin durumunu ve diğer dış servislerin durumlarını kontrol etmek için kullandım. Konfigürasyon genel olarak şu şekildedir:
- Dış servislerin daha önce yazılmış HealthChecks kütüphaneleri (AspNetCore.HealthChecks.SqlServer, .Redis, .Elasticsearch vs.) projeye dahil edilir.
- Çıktıyı daha istediğimiz gibi ayarlamak için [UI.Client](https://www.nuget.org/packages/AspNetCore.HealthChecks.UI.Client) da dahil edilir.
- Serviclerin HealthChecks [konfigürasyonları](XWebAPI/Extensions/ServicesExtensions.cs#L174) eklenir. Bu konfigürasyonlar şu şekildedir:
  - Bağalntı noktası için `connectionString` veya `Uri`
  - Servise göre `healthQuery`
  - Check response' nda servise verilen isim (Bu isim unique olmalı) `name`
  - Servis ulaşılamadığı zaman ki durum değeri `failureStatus`
  - Son olarak tag'ler `tags`
- Healt Check rotasını [Program.cs](/XWebAPI/Program.cs#L106) tanımlıyoruz. aynı zamanda `HealthCheckOptions` ile response' daki diğer ayarlamaları da yapabiliyoruz (Örneğin: Exception'nın hata mesajını ayrıntılı olarak verme gibi). 
- İşlemler tamamlanmış oldu. Artık API' nin `BaseUrl/health` Url'ine istek atığımızda API' nin ve external servislerinin durumunu görebileceğiz.
- Bu projedeki örnek Health Check response:
  ```JSON
    {
      "status": "Healthy",
      "totalDuration": "00:00:00.0030152",
      "entries": {
          "SQL Server Check": {
              "data": {},
              "duration": "00:00:00.0024416",
              "status": "Healthy",
              "tags": [
                  "sql",
                  "sql-server"
              ]
          },
          "Redis Check": {
              "data": {},
              "duration": "00:00:00.0026851",
              "status": "Healthy",
              "tags": [
                  "cache",
                  "redis"
              ]
          },
          "Minio Check": {
              "data": {},
              "duration": "00:00:00.0024188",
              "status": "Healthy",
              "tags": [
                  "file",
                  "minio"
              ]
          },
          "Elasticsearch Check": {
              "data": {},
              "duration": "00:00:00.0029169",
              "status": "Healthy",
              "tags": [
                  "log",
                  "elasticsearch"
              ]
          }
      }
  }

  ````

### Data Validation (Fluent Validation)
Veri tabanına verileri kaydetmeden önce bu verilerin bizim istediğimiz veya veri tabanındaki özelikleriyle uyuşup uyuşmadığını kontrol etmek zorundayız. Bunu yapmasak büyük ihtimale haat alırız. Bu yüzden farklı validasyon yöntemleri mevcutur, isterseniz kendiniz her bir model veya Dto (Data Transfer Object) için ayrı ayrı validasyonlar yazabilirsiniz, veri anotasyonları (Data Annotations) ile bunu yapabilir veya bu işi yapan hazır kütüphanelerden de yararlanabilirsiniz. Ben bu projede güçlü bir kütüphane olan **Fluent Validation** kullandım. Kullanımı şu şekilde:
- [FluentValidation](https://www.nuget.org/packages/FluentValidation) bağımlılığını projeye dahil ediyorsunuz.
- Daha sonra ilgili validator'larımızın nerede olacağını Program.cs' de belirtmemiz gerekiyor. Presentation katmanına  referans için [AssemblyReference](Presentation/AssemblyReference.cs) adında bir class ekeldim ve bunu [Program.cs](/XWebAPI/Program.cs#L55)'de projeye entegre etim.
- Artık tek yapmak gereken `Presentation` katmanında bu validasyon sınıflarını oluşturmak.
- Örneğin kullanıcı kayıt Dto' sunun [validator](Presentation/Validators/User/UserRegisterValidator.cs)'ını incelecek olursak:
  - Öncelikle Fluent Validation' nın generic `AbstractValidator` sınıfını ilgili Dto' muz ile beraber miras alıyoruz.
  - Daha sonra kurucu methodumuz içinde ilgili kuraları belitiyoruz. [Örneğin](Presentation/Validators/User/UserRegisterValidator.cs#L11) `FullName` alanının **NULL** ve **Empty** olamayacağını, uzunluğunun minimum 5, maksimum 100 olabileceğini belirtiyoruz. Eğer bu kuralara uymayan bir değer gelirse fluent validation' un `IsValid` değeri false olacak ve kullanıcıya ister custom ister default hata mesajını dönecektir.  
- Validasyonu menüel veya otomatik yapmak mümkündür. Ben projede manüel validasyonu kullandım. Örneğin kullanıcı kayıt [endpoint](Presentation/Controllers/v1/UserController.cs#L56) i de `[FromServices] IValidator<UserDtoForRegister> validator` şeklinde methoda ekliyoruz. Artık bu endpoint e bir istek yapıldığında `UserDtoForRegister`' nın validator'ı varsa bunu `IValidator<T>` ile enjekte edrek validasyon işlemini manüel olarak gerçekleştirebiliyoruz.
   


### Unit Test (XUnit)
Geliştirilen projenin test edilebilir en küçük parçasının test edilmesi işlemidir. Bu konu hakkında daha önce bir medium makalesi yazdığım için burada anlatmayacağım. İsteyenler [blog yazımı](https://mehmet-eren.medium.com/unit-testing-dda3b10a8a69) okuyabilirler. 



## GitHub Actions
GitHub' da bulunana projenizde gerçekleşen herhangi bir eylemde başka bir işlem tetikleyebildiğiniz bir platformdur. Örneğin repo' nun `main` branch' ine bir commit atıldığında veya bir merge işlemi yapıldığında uygulamayı build edebilirsiniz ya da benim bu projede yaptığım gibi `main` branch' inde yapılan herhangi bir değişilikte unit teslerinizi çalıştırıp bunu durumunu da Read.md dosyanızda badge olarak gösterebilirsiniz. Örnek badge bu dosyanın en üst kısmındadır. Bu işlemleri birleştirmek de mümkündür, örneğin `main` branch' ine bir merge işlemi yapıldığında gidip önce unit tesleri çalıştırıp eğer unit testler geçerse projeyi build edip veya paketleyip başka bir platforma push' layabilirsiniz.
Projeye eklemek şu şekildedir:
- Projeye `.github` ve onun içinede `workflows` adında bir klasör oluşturuyoruz. WorkFlows dosyasına YML formatında iş akışlarımızı ekliyoruz. Örnek dosya [unit-test.yml](.github/workflows/unit-tests.yml):
```YML
  name: build and test # Akışın adı

on:
  push: # main branch' ine bir push veya pull request işlemi yapıldığında ve .cs veya .csproj uzantılı dosyalarda değişiklik olmuşsa çalışacağını belirtiyoruz
  pull_request:
    branches: [ main ]
    paths:
    - '**.cs'
    - '**.csproj'

env:
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-test: # Job tanımlama

    name: build-and-test-${{matrix.os}} # İşlemi farklı işletim sistemleri üzerinde çalıştırma.
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest]

    steps:
    - uses: actions/checkout@v3 # repo' nun dosyalarını işlem yapılacak konuma kopyalar.
    - name: Setup .NET Core
      uses: actions/setup-dotnet@v3 # .Net Core' un yapılandırılması
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: Install dependencies
      run: dotnet restore # Bağımlılıkların yüklenmesi
      
    - name: Build
      run: dotnet build --configuration Release --no-restore # projeyi derleme
    
    - name: Test
      run: dotnet test --no-restore --verbosity normal # testlerin çalıştırılması

```



## File Storage
Projelerde dosya işlemleri birçok şekilde yapılabilir. Bunlar; dosyayı binary olarak veri tabanına kaydetmek, dosyayı proje üzerinde bir `path`' e kaydetmek (en tehlikeli ve old school olanı) veya external bir servisde tutmak. Her senaryonun kendine göre ve proje özelinde avantajları ve dezavantajları vardır (API üzerinde tutmayın !!). Ben doğrudan daha çok kullanıldığını gördüğüm yöntemi kullandım yani dosyaları dış bir serviste tutum. Bu servis Minio, minio open source bir nesne depolama sistemidir. Ayrıca Amazon S3 ile API uyumludur.

Entegrasyonu:
- [Minio](https://www.nuget.org/packages/Minio) bağımlılığını projeye dahil ediyoruz.
- [appsettings.json](/XWebAPI/appsettings.json#L81)' da gerekli configürasyonları giriyoruz (Bunları statik olarak kod içinde de yazabilirsiniz).
- [ServicesExtensions](XWebAPI/Extensions/ServicesExtensions.cs#L47)' da gerekli ayarlamalar yapılır ve [Program.cs](XWebAPI/Program.cs#L63) ' de bu extension eklenir.
- Artık kullanmak istediğiniz serviste `IMinioClient`' ı enjekte etmeniz yeterli olacaktır (Diğer işlemlerin kodları [FileUploadManager](Services/FileUploadManager.cs) ve [FileDownloadManager](Services/FileDownloadManager.cs)' da mevcutur oradaki kodları inceleyebilirsiniz.)