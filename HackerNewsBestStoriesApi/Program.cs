using HackerNewsBestStoriesApi.Models;
using HackerNewsBestStoriesApi.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new() { Title = "Hacker News Best Stories API", Version = "v1" });
});

builder.Services.Configure<HackerNewsOptions>(
    builder.Configuration.GetSection("HackerNews")
);

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();