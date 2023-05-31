using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Environment.SetEnvironmentVariable("Environment", env ?? "");

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables().AddCommandLine(args);
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFlash(flash =>
{
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
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
