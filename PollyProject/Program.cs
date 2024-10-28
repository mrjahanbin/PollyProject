using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading.Tasks;

#region 0
//0
//بدون پولی
//class Program
//{
//    private static readonly HttpClient _httpClient = new HttpClient();

//    static async Task Main(string[] args)
//    {
//        string apiUrl = "https://jsonplaceholder.typicode.com/posts/1";

//        try
//        {
//            // ارسال درخواست HTTP ساده به API
//            Console.WriteLine("Send HTTP...");
//            var response = await _httpClient.GetAsync(apiUrl);
//            response.EnsureSuccessStatusCode(); // اگر کد وضعیت غیر از 200 باشد، استثنا پرتاب می‌شود
//            var content = await response.Content.ReadAsStringAsync();
//            Console.WriteLine("Response:");
//            Console.WriteLine(content);
//        }
//        catch (HttpRequestException ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }
//}

#endregion
#region 1
//class Program
//{
//    private static readonly HttpClient _httpClient = new HttpClient();

//    static async Task Main(string[] args)
//    {
//        string apiUrl = "https://jsonplaceholder.typicode.com/post/1";

//        // سیاست Retry: تلاش مجدد در صورت بروز خطا
//        var retryPolicy = Policy
//            .Handle<HttpRequestException>() // مدیریت خطاهای HttpRequestException
//            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2), // سه بار تلاش مجدد با فاصله ۲ ثانیه
//            (exception, timeSpan, retryCount, context) =>
//            {
//                Console.WriteLine($"Retry {retryCount}: Error - {exception.Message}");
//            });

//        try
//        {
//            // اجرای درخواست HTTP با سیاست Retry
//            await retryPolicy.ExecuteAsync(async () =>
//            {
//                Console.WriteLine("Send HTTP...");
//                var response = await _httpClient.GetAsync(apiUrl);
//                response.EnsureSuccessStatusCode(); // اگر کد وضعیت غیر از 200 باشد، استثنا پرتاب می‌شود
//                var content = await response.Content.ReadAsStringAsync();
//                Console.WriteLine("Response:");
//                Console.WriteLine(content);
//            });
//        }
//        catch (HttpRequestException ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }
//}
#endregion
#region 2
//class Program
//{
//    private static readonly HttpClient _httpClient = new HttpClient();

//    static async Task Main(string[] args)
//    {
//        string apiUrl = "https://jsonplaceholder.typicode.com/post/1";

//        // سیاست Retry: تلاش مجدد در صورت بروز خطا
//        var retryPolicy = Policy
//            .Handle<HttpRequestException>()
//            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2), // سه بار تلاش مجدد با فاصله ۲ ثانیه
//            (exception, timeSpan, retryCount, context) =>
//            {
//                Console.WriteLine($"Retry {retryCount}: Error - {exception.Message}");
//            });

//        // سیاست Circuit Breaker: قطع مدار در صورت بروز خطاهای متوالی
//        var circuitBreakerPolicy = Policy
//            .Handle<HttpRequestException>()
//            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(5), // بعد از ۲ خطای متوالی مدار را برای ۳۰ ثانیه قطع می‌کند
//            onBreak: (exception, duration) =>
//            {
//                Console.WriteLine($"duration: {duration.TotalSeconds} s Error: {exception.Message}");
//            },
//            onReset: () =>
//            {
//                Console.WriteLine("Reset Request");
//            });

//        try
//        {
//            // ترکیب دو سیاست با هم
//            var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

//            // اجرای درخواست HTTP با سیاست‌های ترکیبی
//            await combinedPolicy.ExecuteAsync(async () =>
//            {
//                Console.WriteLine("Send HTTP...");
//                var response = await _httpClient.GetAsync(apiUrl);
//                response.EnsureSuccessStatusCode(); // اگر کد وضعیت غیر از 200 باشد، استثنا پرتاب می‌شود
//                var content = await response.Content.ReadAsStringAsync();
//                Console.WriteLine("Response:");
//                Console.WriteLine(content);
//            });
//        }
//        catch (HttpRequestException ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//        catch (BrokenCircuitException ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}"); 
//            await Task.Delay(TimeSpan.FromSeconds(5)); // انتظار ۵ ثانیه

//        }
//    }
//}
#endregion
#region 3
//class Program
//{
//    private static readonly HttpClient _httpClient = new HttpClient();

//    static async Task Main(string[] args)
//    {
//        string apiUrl = "https://jsonplaceholder.typicode.com/post/1";

//        // سیاست Retry: تلاش مجدد در صورت بروز خطا
//        var retryPolicy = Policy
//            .Handle<HttpRequestException>()
//            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2),
//            (exception, timeSpan, retryCount, context) =>
//            {
//                Console.WriteLine($"Attempt {retryCount}: An error occurred - {exception.Message}");
//            });

//        // سیاست Circuit Breaker: قطع مدار در صورت بروز خطاهای متوالی
//        var circuitBreakerPolicy = Policy
//            .Handle<HttpRequestException>()
//            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(5),
//            onBreak: (exception, duration) =>
//            {
//                Console.WriteLine($"Circuit broken for {duration.TotalSeconds} seconds due to: {exception.Message}");
//            },
//            onReset: () =>
//            {
//                Console.WriteLine("Circuit reset and is now active.");
//            });

//        // سیاست Timeout: قطع درخواست در صورتی که بیش از ۳ ثانیه طول بکشد
//        var timeoutPolicy = Policy
//            .TimeoutAsync(3, TimeoutStrategy.Pessimistic, onTimeoutAsync: (context, timespan, task) =>
//            {
//                Console.WriteLine("Timeout occurred.");
//                return Task.CompletedTask;
//            });

//        // ترکیب سه سیاست با هم
//        var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);

//        int maxAttempts = 5; // حداکثر تعداد تلاش‌ها
//        int attemptCount = 0; // شمارنده تلاش‌ها
//        while (attemptCount < maxAttempts)
//        {
//            attemptCount++;
//            try
//            {
//                // اجرای درخواست HTTP با سیاست‌های ترکیبی
//                await combinedPolicy.ExecuteAsync(async () =>
//                {
//                    Console.WriteLine("Sending HTTP request...");
//                    // اضافه کردن یک تأخیر مصنوعی برای شبیه‌سازی تایم‌اوت
//                    await Task.Delay(5000); // تاخیر ۵ ثانیه‌ای
//                    var response = await _httpClient.GetAsync(apiUrl);
//                    response.EnsureSuccessStatusCode();
//                    var content = await response.Content.ReadAsStringAsync();
//                    Console.WriteLine("Response received:");
//                    Console.WriteLine(content);
//                });
//            }
//            catch (HttpRequestException ex)
//            {
//                Console.WriteLine($"HTTP request failed: {ex.Message}");
//            }
//            catch (BrokenCircuitException ex)
//            {
//                Console.WriteLine($"Circuit is broken and request was not sent: {ex.Message}");

//                // صبر برای مدت زمان مدار قطع‌شده
//                Console.WriteLine("Waiting for circuit to reset...");
//                await Task.Delay(TimeSpan.FromSeconds(5));
//            }
//            catch (TimeoutRejectedException)
//            {
//                Console.WriteLine("Request exceeded timeout and was aborted.");
//            }
//        }
//    }
//}
#endregion
#region 4
//class Program
//{
//    private static readonly HttpClient _httpClient = new HttpClient();

//    static async Task Main(string[] args)
//    {
//        string apiUrl = "https://jsonplaceholder.typicode.com/posts/1";

//        // سیاست Retry: تلاش مجدد در صورت بروز خطا
//        var retryPolicy = Policy
//            .Handle<HttpRequestException>()
//            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2),
//            (exception, timeSpan, retryCount, context) =>
//            {
//                Console.WriteLine($"Attempt {retryCount}: An error occurred - {exception.Message}");
//            });

//        // سیاست Circuit Breaker: قطع مدار در صورت بروز خطاهای متوالی
//        var circuitBreakerPolicy = Policy
//            .Handle<HttpRequestException>()
//            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(5),
//            onBreak: (exception, duration) =>
//            {
//                Console.WriteLine($"Circuit broken for {duration.TotalSeconds} seconds due to: {exception.Message}");
//            },
//            onReset: () =>
//            {
//                Console.WriteLine("Circuit reset and is now active.");
//            });

//        // سیاست Timeout: قطع درخواست در صورتی که بیش از ۳ ثانیه طول بکشد
//        var timeoutPolicy = Policy
//            .TimeoutAsync(3, TimeoutStrategy.Optimistic, onTimeoutAsync: (context, timespan, task) =>
//            {
//                Console.WriteLine("Timeout occurred.");
//                return Task.CompletedTask;
//            });

//        // سیاست Fallback: ارائه پاسخ جایگزین در صورت شکست درخواست
//        var fallbackPolicy = Policy<string>
//            .Handle<Exception>()
//            .FallbackAsync(
//                fallbackValue: "{\"userId\": 1, \"id\": 1, \"title\": \"Fallback response\", \"body\": \"This is a fallback response.\"}",
//                onFallbackAsync: (exception, context) =>
//                {
//                    Console.WriteLine("Fallback executed due to an error: " + exception.Exception.Message);
//                    return Task.CompletedTask;
//                });

//        // ترکیب چهار سیاست با هم
//        var combinedPolicy = fallbackPolicy.WrapAsync(timeoutPolicy)
//                                           .WrapAsync(circuitBreakerPolicy)
//                                           .WrapAsync(retryPolicy);

//        int maxAttempts = 5; // حداکثر تعداد تلاش‌ها
//        int attemptCount = 0; // شمارنده تلاش‌ها

//        while (attemptCount < maxAttempts)
//        {
//            attemptCount++;
//            try
//            {
//                // اجرای درخواست HTTP با سیاست‌های ترکیبی
//                var result = await combinedPolicy.ExecuteAsync(async () =>
//                {
//                    Console.WriteLine("Sending HTTP request...");

//                    // اضافه کردن یک تأخیر مصنوعی برای شبیه‌سازی تایم‌اوت
//                    await Task.Delay(5000); // تاخیر ۵ ثانیه‌ای

//                    var response = await _httpClient.GetAsync(apiUrl);
//                    response.EnsureSuccessStatusCode();
//                    var content = await response.Content.ReadAsStringAsync();
//                    Console.WriteLine("Response received:");
//                    return content;
//                });

//                Console.WriteLine("Result:");
//                Console.WriteLine(result);

//                break; // در صورت موفقیت‌آمیز بودن درخواست، حلقه را متوقف کنید
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
//            }
//        }

//        Console.WriteLine("Max attempts reached or successful response received. Stopping the application.");
//    }
//}
#endregion
#region 5
//class Program
//{
//    private static readonly HttpClient _httpClient = new HttpClient();

//    static async Task Main(string[] args)
//    {
//        string apiUrl = "https://jsonplaceholder.typicode.com/posts/1";

//        // سیاست Retry: تلاش مجدد در صورت بروز خطا
//        var retryPolicy = Policy
//            .Handle<HttpRequestException>()
//            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2),
//            (exception, timeSpan, retryCount, context) =>
//            {
//                Console.WriteLine($"Attempt {retryCount}: An error occurred - {exception.Message}");
//            });

//        // سیاست Circuit Breaker: قطع مدار در صورت بروز خطاهای متوالی
//        var circuitBreakerPolicy = Policy
//            .Handle<HttpRequestException>()
//            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(5),
//            onBreak: (exception, duration) =>
//            {
//                Console.WriteLine($"Circuit broken for {duration.TotalSeconds} seconds due to: {exception.Message}");
//            },
//            onReset: () =>
//            {
//                Console.WriteLine("Circuit reset and is now active.");
//            });

//        // سیاست Timeout: قطع درخواست در صورتی که بیش از ۳ ثانیه طول بکشد
//        var timeoutPolicy = Policy
//            .TimeoutAsync(3, TimeoutStrategy.Optimistic, onTimeoutAsync: (context, timespan, task) =>
//            {
//                Console.WriteLine("Timeout occurred.");
//                return Task.CompletedTask;
//            });

//        // سیاست Fallback: ارائه پاسخ جایگزین در صورت شکست درخواست
//        var fallbackPolicy = Policy<string>
//            .Handle<Exception>()
//            .FallbackAsync(
//                fallbackValue: "{\"userId\": 1, \"id\": 1, \"title\": \"Fallback response\", \"body\": \"This is a fallback response.\"}",
//                onFallbackAsync: (exception, context) =>
//                {
//                    Console.WriteLine("Fallback executed due to an error: " + exception.Exception.Message);
//                    return Task.CompletedTask;
//                });

//        // سیاست Bulkhead: محدود کردن تعداد درخواست‌های همزمان به حداکثر ۲ درخواست
//        var bulkheadPolicy = Policy.BulkheadAsync<string>(2, int.MaxValue, onBulkheadRejectedAsync: context =>
//        {
//            Console.WriteLine("Bulkhead limit reached. Request was rejected.");
//            return Task.CompletedTask;
//        });

//        // ترکیب پنج سیاست با هم
//        var combinedPolicy = fallbackPolicy.WrapAsync(bulkheadPolicy)
//                                           .WrapAsync(timeoutPolicy)
//                                           .WrapAsync(circuitBreakerPolicy)
//                                           .WrapAsync(retryPolicy);

//        // ایجاد لیست Task‌ها برای اجرای موازی درخواست‌ها
//        var tasks = new List<Task>();
//        for (int i = 0; i < 5; i++) // ایجاد ۵ درخواست همزمان
//        {
//            tasks.Add(SendRequestAsync(combinedPolicy, apiUrl, i + 1));
//        }

//        // اجرای تمامی Task‌ها به صورت موازی و انتظار برای اتمام آن‌ها
//        await Task.WhenAll(tasks);

//        Console.WriteLine("All requests completed.");
//    }

//    // متد ارسال درخواست HTTP با سیاست‌های ترکیبی
//    private static async Task SendRequestAsync(IAsyncPolicy<string> policy, string url, int requestNumber)
//    {
//        try
//        {
//            var result = await policy.ExecuteAsync(async () =>
//            {
//                Console.WriteLine($"Request {requestNumber}: Sending HTTP request...");

//                var response = await _httpClient.GetAsync(url);
//                response.EnsureSuccessStatusCode();
//                var content = await response.Content.ReadAsStringAsync();
//                Console.WriteLine($"Request {requestNumber}: Response received");
//                return content;
//            });

//            Console.WriteLine($"Request {requestNumber}: Result:");
//            Console.WriteLine(result);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Request {requestNumber}: An error occurred: {ex.Message}");
//        }
//    }
//}
#endregion
#region 6
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Fallback;
using Polly.Bulkhead;
using Polly.Caching;
using Polly.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        string apiUrl = "https://jsonplaceholder.typicode.com/posts/1";

        // سیاست Retry: تلاش مجدد در صورت بروز خطا
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2),
            (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Attempt {retryCount}: An error occurred - {exception.Message}");
            });

        // سیاست Circuit Breaker: قطع مدار در صورت بروز خطاهای متوالی
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(5),
            onBreak: (exception, duration) =>
            {
                Console.WriteLine($"Circuit broken for {duration.TotalSeconds} seconds due to: {exception.Message}");
            },
            onReset: () =>
            {
                Console.WriteLine("Circuit reset and is now active.");
            });

        // سیاست Timeout: قطع درخواست در صورتی که بیش از ۳ ثانیه طول بکشد
        var timeoutPolicy = Policy
            .TimeoutAsync(3, TimeoutStrategy.Optimistic, onTimeoutAsync: (context, timespan, task) =>
            {
                Console.WriteLine("Timeout occurred.");
                return Task.CompletedTask;
            });

        // سیاست Fallback: ارائه پاسخ جایگزین در صورت شکست درخواست
        var fallbackPolicy = Policy<string>
            .Handle<Exception>()
            .FallbackAsync(
                fallbackValue: "{\"userId\": 1, \"id\": 1, \"title\": \"Fallback response\", \"body\": \"This is a fallback response.\"}",
                onFallbackAsync: (exception, context) =>
                {
                    Console.WriteLine("Fallback executed due to an error: " + exception.Exception.Message);
                    return Task.CompletedTask;
                });

        // سیاست Bulkhead: محدود کردن تعداد درخواست‌های همزمان به حداکثر ۲ درخواست
        var bulkheadPolicy = Policy.BulkheadAsync<string>(2, int.MaxValue, onBulkheadRejectedAsync: context =>
        {
            Console.WriteLine("Bulkhead limit reached. Request was rejected.");
            return Task.CompletedTask;
        });

        // تنظیم سیاست Cache با استفاده از MemoryCache
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cacheProvider = new MemoryCacheProvider(memoryCache);
        var cachePolicy = Policy.CacheAsync<string>(cacheProvider, TimeSpan.FromMinutes(1));

        // ترکیب شش سیاست با هم
        var combinedPolicy = fallbackPolicy.WrapAsync(cachePolicy)
                                           .WrapAsync(bulkheadPolicy)
                                           .WrapAsync(timeoutPolicy)
                                           .WrapAsync(circuitBreakerPolicy)
                                           .WrapAsync(retryPolicy);

        // اجرای چندین درخواست برای تست Cache Policy
        for (int i = 1; i <= 3; i++)
        {
            await SendRequestAsync(combinedPolicy, apiUrl, i);
        }

        Console.WriteLine("All requests completed.");
    }

    // متد ارسال درخواست HTTP با سیاست‌های ترکیبی
    private static async Task SendRequestAsync(IAsyncPolicy<string> policy, string url, int requestNumber)
    {
        try
        {
            var result = await policy.ExecuteAsync(async (context) =>
            {
                Console.WriteLine($"Request {requestNumber}: Sending HTTP request...");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Request {requestNumber}: Response received");
                return content;
            }, new Context(url)); // Context برای مدیریت Cache Policy

            Console.WriteLine($"Request {requestNumber}: Result:");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request {requestNumber}: An error occurred: {ex.Message}");
        }
    }
}

#endregion