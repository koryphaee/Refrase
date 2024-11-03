using Refrase.Api;
using Refrase.Core;
using Refrase.Server;
using Refrase.Server.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddModel();
builder.Services.AddCore();
builder.Services.AddApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorComponents();

builder.WebHost.ConfigureKestrel(k => k.Limits.MaxRequestBodySize = null);

await using WebApplication app = builder.Build();

app.UseAntiforgery();
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();
app.MapApi();
app.MapRazorComponents<App>();

await app.Migrate();
await app.RunAsync();
