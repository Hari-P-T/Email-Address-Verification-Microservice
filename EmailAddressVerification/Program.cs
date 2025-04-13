using EmailAddressVerification.Services;
using EmailAddressVerificationAPI.Models;
using EmailAddressVerificationAPI.Services;
using System.Threading.RateLimiting;
using EmailAddressVerification.Policies;
using Microsoft.AspNetCore.RateLimiting;



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
    options.AddPolicy("PerIpPolicy", context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Maintain separate limiter per IP
        var limiter = IpRateLimitStore.GetLimiterForIp(ipAddress);
        return RateLimitPartition.Get(ipAddress, _ => limiter);

    });
});

//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//        RateLimitPartition.GetConcurrencyLimiter(
//            partitionKey: httpContext.Request.Headers.Host.ToString(),
//            factory: partition => new ConcurrencyLimiterOptions
//            {
//                PermitLimit = 20,           // Max concurrent requests
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                QueueLimit = 100            // Optional: max queued requests
//            }));
//});


var app = builder.Build();

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
//app.UseRateLimiter();

app.MapControllers();
app.Run();