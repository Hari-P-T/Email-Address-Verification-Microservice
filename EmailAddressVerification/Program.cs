// Program.cs
using EmailAddressVerificationAPI.Services;
using EmailAddressVerificationAPI.Models;
using System.Threading.RateLimiting;
using EmailAddressVerificationAPI.Policies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SmtpServerVerification>();
builder.Services.AddSingleton<WhiteListedEmailProvider>();
builder.Services.AddSingleton<TopLevelDomainVerification>();
builder.Services.AddSingleton<VulgarWordSearch>();
builder.Services.AddSingleton<DisposableDomainsCheck>();
builder.Services.AddSingleton<DomainVerification>();
builder.Services.AddSingleton<ResponseDTO>();
builder.Services.AddSingleton<List<ChecklistElementDTO>>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PerIpPolicy", httpContext =>
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetTokenBucketLimiter(ipAddress, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 2,                             
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0,                     
            ReplenishmentPeriod = TimeSpan.FromSeconds(100), 
            TokensPerPeriod = 2,                         
            AutoReplenishment = true            
        });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseRateLimiter();

app.UseAuthorization();

//app.MapControllers().RequireRateLimiting("PerIpPolicy");

app.Run();
