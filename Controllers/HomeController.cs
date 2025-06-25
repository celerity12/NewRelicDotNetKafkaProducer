using HellowWebAppSR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using NewRelic.Api.Agent;
using System.Runtime.CompilerServices;

namespace HellowWebAppSR.Controllers
{
    public class HomeController : Controller
    {
        #region private fields
        private readonly ILogger<HomeController> _logger;
        private readonly string _apiKey = "8084d65919a02cdcac6d28a040ad26ad";
        private readonly HttpClient _httpClient;
        private const string KafkaBootstrapServers = "localhost:9092";
        private const string KafkaTopic = "demo-topic";
        private const string KafkaGroupId = "demo-group";
        #endregion
        public HomeController(ILogger<HomeController> logger)
        {
            this._logger = logger;
            this._httpClient = new HttpClient();
            _logger.LogInformation("HomeController initialized.");
        }
        public IActionResult Index()
        {
            string language = "english";
            string version = "v1";
            Random random = new Random();
            int langIndex = random.Next(0, 2);
            int versionIndex = random.Next(0, 3);
            if (langIndex == 1)
            {
                language = "spanish";
            }
            if (versionIndex == 1)
            {
                version = "v2";
            }
            else if (versionIndex == 2)
            {
                version = "v3";
            }
            return RedirectToAction("IndexWithLanguageAndVersion", new { lang = language, version = version });
        }
        [Route("/Home/Index/{lang}/{version}/index.html")]
        public IActionResult IndexWithLanguageAndVersion(string lang, string version)
        {
            Random random = new Random();
            long luckyNumber = random.Next(10000000, 99999999);
            luckyNumber = luckyNumber * 100000000 + random.Next(10000000, 99999999);
            int delay = random.Next(400, 1000);
            int firstFourDigits = (int)(luckyNumber / 1000000000000);
            long calculationResult = (long)Math.Pow(firstFourDigits, 4);
            calculationResult = (calculationResult * firstFourDigits) / 123;
            calculationResult += calculationResult % 456;
            calculationResult *= 789;
            calculationResult -= (long)Math.Sqrt(Math.Abs(calculationResult));
            ViewBag.LuckyNumber = luckyNumber;
            ViewBag.Delay = delay;
            ViewBag.CalculationResult = calculationResult;
            ViewBag.Language = lang;
            ViewBag.Version = version;
            string viewName = "Index";
            if (lang == "spanish")
            {
                viewName = "Index.es";
            }
            else if (lang == "english")
            {
                viewName = "Index.en";
            }
            return View(viewName);
        }
        public IActionResult Privacy()
        {
            Random random = new Random();
            int randomNumber1 = random.Next(1, 3);
            int randomNumber2 = random.Next(100, 999999);
            string version = "v1";
            Random random_version = new Random();
            int versionIndex = random_version.Next(0, 3);
            if (versionIndex == 1)
            {
                version = "v2";
            }
            else if (versionIndex == 2)
            {
                version = "v3";
            }
            return RedirectToAction("PrivacyWithNumbers", new { num1 = randomNumber1, num2 = randomNumber2, version = version });
        }
        [Route("/Home/Privacy/{num1}/SRHW/{num2}/{version}/privacy.html")]
        public async Task<IActionResult> PrivacyWithNumbers(int num1, int num2, string version)
        {
            Random random = new Random();
            if (random.NextDouble() < 0.5)
            {
                try
                {
                    int exceptionType = random.Next(3);
                    switch (exceptionType)
                    {
                        case 0:
                            throw new DivideByZeroException("Random DivideByZeroException occurred in Privacy!");
                        case 1:
                            throw new NullReferenceException("Random NullReferenceException occurred in Privacy!");
                        case 2:
                            throw new Exception("Random generic Exception occurred in Privacy!");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"An exception occurred: {ex.Message}";
                    return View("Privacy");
                }
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("https://www.bing.com");
                    response.EnsureSuccessStatusCode();
                    string html = await response.Content.ReadAsStringAsync();
                    string snippet = html.Substring(0, Math.Min(5000, html.Length));
                    ViewBag.CnnSnippet = snippet;
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.CnnError = $"Error fetching CNN: {ex.Message}";
            }
            ViewBag.Num1 = num1;
            ViewBag.Num2 = num2;
            ViewBag.Version = version;
            return View("Privacy");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult GetWeatherProducer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetWeatherProducer(ZipCodeInputModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueId = Guid.NewGuid().ToString();
                return RedirectToAction("ShowWeather", new { uniqueId = uniqueId, zipCode = model.ZipCode });
            }
            return View(model);
        }
        [Route("Home/ShowWeather/CITY/{zipCode}")]
        public async Task<IActionResult> ShowWeather(string uniqueId, string zipCode)
        {
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?zip={zipCode},us&appid={_apiKey}&units=imperial";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var weatherData = JsonConvert.DeserializeObject<WeatherData>(responseBody);
                ViewBag.UniqueId = uniqueId;
                return View(weatherData);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error fetching weather data.");
                return View("GetWeatherProducer");
            }
            catch (JsonException)
            {
                ModelState.AddModelError(string.Empty, "Error processing weather data.");
                return View("GetWeatherProducer");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
    
        public async Task<IActionResult> ProduceKafkaMessage([FromBody] KafkaMessageModel model)
        {
            string message = model.Message ?? "Default message";
            var config = new ProducerConfig { BootstrapServers = KafkaBootstrapServers };
            var sw = Stopwatch.StartNew();
            long dummy = 0;
            while (sw.Elapsed < TimeSpan.FromSeconds(2))
            {
                for (int i = 0; i < 10_000_000; i++)
                {
                    dummy += i % 7;
                }
            }
            return await CreateMessage(message, config);
        }
        [Trace]
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async Task<IActionResult> CreateMessage(string message, ProducerConfig config)
        {
            try
            {
                string messag = Fn1(message).Substring(0, 25);
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    var dr = await producer.ProduceAsync(KafkaTopic, new Message<Null, string> { Value = message });
                    _logger.LogInformation($"Produced Kafka message: {message} to topic: {KafkaTopic} at offset: {dr.Offset}");
                }
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, $"Kafka produce error for topic '{KafkaTopic}': {ex.Error.Reason}");
                return Json(new { success = false, error = ex.Error.Reason });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"General error producing Kafka message to topic '{KafkaTopic}'.");
                return Json(new { success = false, error = ex.Message });
            }
            return Json(new { success = true });
        }

        #region utility functions

        [Trace]
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private string Fn1(string msg)
        {
            msg += " | fn1 called";
            msg += Fn2(msg);
            NewRelic.Api.Agent.ITransaction transaction = NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction;
            transaction.AddCustomAttribute("Attr_Fn1_Producer", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return msg;
        }
        private string Fn2(string msg)
        {
            //Custom attribute for Fn2
            NewRelic.Api.Agent.ITransaction transaction = NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction;
            transaction.AddCustomAttribute("Attr_Fn2_Producer", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            msg += " | fn2 called";
            msg += Fn3(msg);
            return msg;
        }
        private string Fn3(string msg)
        {
            msg += " | fn3 called";
            msg += Fn4(msg);
            return msg;
        }
        private string Fn4(string msg)
        {
            //Custom Event New Relic
            // Create a dictionary of attributes to send with the custom event
        var eventAttributes = new Dictionary<string, object>
        {
            { "ProucerCustomEvent", "CE1" },
            { "ProducerCE2", "CE2" },
            { "message", msg },
            { "timestamp", DateTime.UtcNow }
        };

        // Record the custom event to New Relic
        NewRelic.Api.Agent.NewRelic.RecordCustomEvent("PRODUCER_CUSTOM_EVENT", eventAttributes);
            msg += " | fn4 called";
            msg += Fn5(msg);
            return msg;
        }
        private string Fn5(string msg)
        {
            msg += " | fn5 called";
            return msg;
        }

        [Trace]
        private async Task<string> JobFn1(string msg)
        {
            await Task.Delay(200);
            msg += " | JobFn1 called";
            msg = await JobFn2(msg);
            return msg;
        }

        [Trace]
        private async Task<string> JobFn2(string msg)
        {
            await Task.Delay(200);
            msg += " | JobFn2 called";
            msg = await JobFn3(msg);
            return msg;
        }

        [Trace]
        private async Task<string> JobFn3(string msg)
        {
            await Task.Delay(200);
            msg += " | JobFn3 called";
            msg = await JobFn4(msg);
            return msg;
        }

        [Trace]
        private async Task<string> JobFn4(string msg)
        {
            await Task.Delay(200);
            msg += " | JobFn4 called";
            msg = await JobFn5(msg);
            return msg;
        }

        [Trace]
        private async Task<string> JobFn5(string msg)
        {
            await Task.Delay(200);
            msg += " | JobFn5 called";
            NewRelic.Api.Agent.ITransaction transaction = NewRelic.Api.Agent.NewRelic.GetAgent().CurrentTransaction;
            transaction.AddCustomAttribute("Attr_JobFn5_Producer", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return msg;
        }
        #endregion

        [HttpGet("Home/fire-and-forget")]
        [Transaction]
        public IActionResult StartBackgroundTask()
        {
            Task.Run(async () =>
            {
                await Task.Delay(20000);
                _logger.LogInformation("Background task completed after 20 seconds.");
                string result = await JobFn1("Fire and Forget");
                result += $"Fire and Forget task started! Final result: {result}";
                result += " at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _logger.LogInformation(result);
            });
            return Json(new { message = "StartBackgroundTask started (fire-and-forget, running in background for 20 seconds)." });
        }
        [HttpPost]
        public async Task<IActionResult> RunInvisibleBackgroundProcess()
        {
            string result = await JobFn1("Background Process");
            await Task.Delay(10000);
            result += $"Background process completed! Final result: {result}";
            result += " at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return Json(new { message = result });
        }

    }

    public class KafkaMessageModel
    {
        public string? Message { get; set; }
    }
}

