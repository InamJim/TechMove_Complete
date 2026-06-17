using TechMove.ApiServices;
using TechMoveServices.Interfaces;
using TechMoveServices.Services;
using TechMoveServices.Strategies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var apiBase = builder.Configuration["ApiSettings:BaseUrl"]
              ?? "http://localhost:5180/";

builder.Services.AddHttpClient("TokenClient", client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddScoped<ApiTokenService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var config = sp.GetRequiredService<IConfiguration>();
    var client = factory.CreateClient("TokenClient");
    return new ApiTokenService(client, config);
});

builder.Services.AddHttpClient<ContractApiService>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddHttpClient<ClientApiService>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddHttpClient<ServiceRequestApiService>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddHttpClient<DashboardApiService>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddScoped<ICurrencyStrategy, USDToZARStrategy>();
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddHttpClient<IExchangeRateApiService, ExchangeRateApiService>();

var app = builder.Build();

app.UseMiddleware<TechMove.Middleware.ExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
