using SearchApp.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddAppLogging();
builder.Services.AddAppSettings(builder.Configuration);
builder.Services.AddAppHttpClients(builder.Configuration);
builder.Services.AddAppStorage(builder.Configuration);
builder.Services.AddAppCoreServices();

var app = builder.Build();

app.UseAppMiddleware();

app.Run();