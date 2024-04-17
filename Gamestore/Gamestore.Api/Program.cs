using System.Reflection;
using Gamestore.Api.Filters;
using Gamestore.Api.Middleware;
using Gamestore.Api.Services;
using Gamestore.Api.Services.Interfaces;
using Gamestore.Database.Dbcontext;
using Gamestore.Database.Entities.MongoDB;
using Gamestore.Database.Repositories;
using Gamestore.Database.Repositories.Interfaces;
using Gamestore.Database.Services;
using Gamestore.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using Serilog.Exceptions;
using Log = Serilog.Log;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHttpClient<IPaymentService, PaymentService>();

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IMongoOrderRepository, MongoOrderRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IShipperRepository, ShipperRepository>();

builder.Services.AddScoped<IDataBaseLogger, DataBaseLogger>();

builder.Services.AddScoped<LoggingActionFilter>();
builder.Services.AddScoped<GlobalExceptionHandler>();

builder.Services.AddSingleton(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>();
    return new MongoContext(mongoSettings);
});

builder.Services.AddSingleton(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var client = new MongoClient(mongoSettings.ConnectionString);
    var database = client.GetDatabase(mongoSettings.Database);

    return database.GetCollection<ProductOrder>("ProductOrders");
});

builder.Services.AddSingleton(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var client = new MongoClient(mongoSettings.ConnectionString);
    var database = client.GetDatabase(mongoSettings.Database);

    return database.GetCollection<ProductShipper>("ProductShippers");
});

builder.Services.AddDbContext<GamestoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Gamestore.Api")));

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDatabase"));

builder.Services.AddSingleton(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>();
    return new MongoContext(mongoSettings);
});

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionHandler>();
    options.Filters.Add<LoggingActionFilter>();
}).AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<IpAddressLoggingMiddleware>();

app.UseMiddleware<PerformanceLoggingMiddleware>();

app.Run();
