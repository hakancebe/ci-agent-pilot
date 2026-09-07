using CiPilot.Core;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Deploy sırasında ortamdan gelmesi ZORUNLU ayar. Eksikse uygulama hiç ayağa
// kalkmaz: yanlış yapılandırmayla sessizce çalışmaktansa açılışta gürültülü
// şekilde durmak tercih edildi (fail fast).
var connectionString = Environment.GetEnvironmentVariable("CIPILOT_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "CIPILOT_CONNECTION_STRING ortam değişkeni tanımlı değil. "
        + "Deploy adımında bu değerin container'a geçirilmesi gerekiyor.");
}

// İkinci zorunlu ayar. main'deki cd.yml bunu container'a geçirmiyor,
// dolayısıyla bu daldaki deploy kasıtlı olarak patlayacak.
var apiKey = Environment.GetEnvironmentVariable("CIPILOT_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException(
        "CIPILOT_API_KEY ortam değişkeni tanımlı değil. "
        + "Deploy adımında bu değerin container'a geçirilmesi gerekiyor.");
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/add", (int a, int b) => Results.Ok(new { result = new Calculator().Add(a, b) }));

app.Run();
