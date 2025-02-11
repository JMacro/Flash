using Flash.Extensions.Office;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Flash.Extensions.Cache;
using Flash.Widgets.Controllers;
using Microsoft.EntityFrameworkCore;
using Flash.Widgets.DbContexts;
using Flash.Widgets.Configures;
using Microsoft.Extensions.Options;
using Flash.Widgets.Models.TaobaoUtils;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Environment.SetEnvironmentVariable("Environment", env ?? "");

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables()
    .AddJsonFileEx("Config/redis.json")
    .AddJsonFileEx("Config/mysql.json")
    .AddJsonFileEx("Config/kuaidi100.json")
    .AddJsonFileEx("Config/expressCode.json")
    .AddJsonFileEx("Config/skuCode.json")
    .AddJsonFileEx("Config/brandUnitPrice.json")
    .AddJsonFileEx("Config/recurringjob.json")
    .AddCommandLine(args);
    builder.Logging.AddLog4Net("Config/log4net.xml", true).AddDebug().AddConsole();

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
    options.SerializerSettings.Converters.Add(new NumberConverter(NumberConverterShip.Int64));
}).AddJsonOptions(config =>
{
    config.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.Configure<ExpressCodeConfigure>(builder.Configuration);
builder.Services.Configure<SkuCodeConfigure>(builder.Configuration);
builder.Services.Configure<CostConfig>(builder.Configuration.GetSection("BrandUnitPrices"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFlash(flash =>
{
    flash.AddUniqueIdGenerator(setup =>
    {
        setup.CenterId = 0;
        setup.UseStaticWorkIdCreateStrategy(0);
    });

    flash.AddOffice(setting =>
    {
        setting.WithDefaultExcelSetting(new SheetSetting
        {
            IsAutoNumber = true
        });
    }, setup =>
    {
        setup.UseNpoi();
    });

    var host = Environment.GetEnvironmentVariable("Redis_Host");
    var password = Environment.GetEnvironmentVariable("Redis_Password");

    flash.AddCache(cache =>
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            host = builder.Configuration["Redis:ServerHost"];
            password = builder.Configuration["Redis:Password"];
        }

        cache.UseRedis(option =>
        {
            option.WithNumberOfConnections(5)
            .WithWriteServerList(host)
            .WithReadServerList(host)
            .WithDb(0)
            .WithDistributedLock(true, false)
            .WithPassword(password)
            .WithKeyPrefix("JMacro:Flash:Tests");
        });
    });

    flash.AddORM(orm =>
    {
        orm.UseEntityFramework(option =>
        {
            var connection = builder.Configuration["ConnectionString:TaoBao"];

            option.RegisterDbContexts<TaoBaoDbContext, TaoBaoMigrationAssembly>(connection, builder.Configuration);
        });
    });

    flash.AddResilientHttpClient((aorign, option) =>
    {
        option.DurationSecondsOfBreak = 30;
        option.ExceptionsAllowedBeforeBreaking = 5;
        option.RetryCount = 5;
        option.TimeoutMillseconds = 10000;
    });

    flash.AddJob(job =>
    {
        job.UseHangfire();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
Console.WriteLine($"IsDevelopment={app.Environment.IsDevelopment()}");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFlash(flash =>
{
    var cache = flash.ApplicationBuilder.ApplicationServices.GetService<ICacheManager>();
    var optionsSkuCode = flash.ApplicationBuilder.ApplicationServices.GetService<IOptionsMonitor<SkuCodeConfigure>>();
    if (cache != null && optionsSkuCode != null)
    {
        cache.StringSet("SkuCodes", optionsSkuCode.CurrentValue.SkuCodes);
    }

    flash.UseHangfire(setup =>
    {
        setup.DashboardPath = "/hangfire";
    });

});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
