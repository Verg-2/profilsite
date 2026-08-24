using KadirPortfolio.Api.Models;
using KadirPortfolio.Api.Services;
using KadirPortfolio.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Load .env variables
DotNetEnv.Env.Load();

// Override configuration with .env values
builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Configuration["TelegramAyarlari:BotToken"] = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? builder.Configuration["TelegramAyarlari:BotToken"];
builder.Configuration["TelegramAyarlari:ChatId"] = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID") ?? builder.Configuration["TelegramAyarlari:ChatId"];
builder.Configuration["AdminSettings:DefaultEmail"] = Environment.GetEnvironmentVariable("ADMIN_DEFAULT_EMAIL") ?? builder.Configuration["AdminSettings:DefaultEmail"];
builder.Configuration["AdminSettings:DefaultPassword"] = Environment.GetEnvironmentVariable("ADMIN_DEFAULT_PASSWORD") ?? builder.Configuration["AdminSettings:DefaultPassword"];
builder.Configuration["YandexCaptcha:SecretKey"] = Environment.GetEnvironmentVariable("YANDEX_CAPTCHA_SECRET") ?? builder.Configuration["YandexCaptcha:SecretKey"];
builder.Configuration["EmailSettings:Email"] = Environment.GetEnvironmentVariable("EMAIL_SENDER_ADDRESS") ?? builder.Configuration["EmailSettings:Email"];
builder.Configuration["EmailSettings:Password"] = Environment.GetEnvironmentVariable("EMAIL_SENDER_PASSWORD") ?? builder.Configuration["EmailSettings:Password"];
builder.Configuration["Jwt:Key"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? builder.Configuration["Jwt:Key"];

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10485760; // 10 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10 MB
});

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- PORT DAYATMASI OLAN SATIRI SİLDİK ---

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueCorsPolicy", corsBuilder =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Geliştirme ortamında (Localhost veya yerel ağ IP'leri ile telefondan test için) her yere açık
            corsBuilder.SetIsOriginAllowed(origin => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
        else
        {
            // Production ortamında Vercel gibi her adresten gelen isteklere izin veriyoruz
            corsBuilder.SetIsOriginAllowed(origin => true) 
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
    });
});
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "KadirPortfolio",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "KadirPortfolioUI",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Secret Key is missing in .env!"))),
            ValidAlgorithms = new[] { Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256 }
        };
    });

builder.Services.Configure<TelegramAyarlari>(
    builder.Configuration.GetSection("TelegramAyarlari"));

builder.Services.AddHttpClient();
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<IScannerService, ScannerService>();

builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<ICaptchaService, CaptchaService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddHttpClient<IAiTranslationService, GeminiTranslationService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(10); // Yapay zeka büyük kod analizleri 100 saniyeden uzun sürebilir
}).ConfigurePrimaryHttpMessageHandler(() => KadirPortfolio.Api.Services.SafeHttpClientHandler.Create());

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("IletisimLimiti", httpContext =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("AuthLimiter", httpContext =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
        await context.HttpContext.Response.WriteAsync(
            "{\"success\":false,\"mesaj\":\"Çok fazla mesaj gönderdiniz. Lütfen 1 dakika sonra tekrar deneyin.\"}",
            token
        );
    };
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });
builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });

var app = builder.Build();

var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedOptions.KnownProxies.Clear();
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Add(System.Net.IPAddress.Parse("127.0.0.1"));
app.UseForwardedHeaders(forwardedOptions);

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); // Ensure database is created/migrated
    DataSeeder.Seed(context, builder.Configuration);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Prod ortamında hata detaylarını (stack trace) dışarı sızdırmamak için global hata yakalayıcı
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\": \"Sunucu tarafında beklenmeyen bir hata oluştu.\"}");
        });
    });
}


// Security Headers Middleware (Clickjacking, MIME Sniffing, XSS protection, CSP, vb.)
app.Use(async (context, next) =>
{
    // Clickjacking Koruması (Siteyi iframe içine almayı engeller)
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    // İçerik Tipi Koklama Koruması
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    // XSS Koruması
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    // Sadece HTTPS kullanımını zorlama (HSTS - Production için)
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    
    string nonce = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16));
    context.Items["CSP_Nonce"] = nonce;

    // Content-Security-Policy (CSP) - Zararlı script enjeksiyonunu engeller
    context.Response.Headers.Append("Content-Security-Policy", $"default-src 'self'; img-src 'self' data: https:; script-src 'self' 'nonce-{nonce}'; style-src 'self' 'unsafe-inline'");
    
    // Referrer-Policy - Hassas URL'lerin sızmasını engeller
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    // Permissions-Policy - Donanım özelliklerine (kamera, mikrofon vb.) erişimi kapatır
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=(), payment=()");
    
    await next();
});

app.UseResponseCompression();
app.UseCors("VueCorsPolicy");
app.UseStaticFiles(); // Added to serve uploaded images from wwwroot
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();