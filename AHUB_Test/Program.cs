using AHUB_Test.Config;
using AHUB_Test.Extensions;
using DomainLayer.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories;
using ServicesLayer.IServices;
using ServicesLayer.Services;
using ServicesLayer.Services.Settings;
using ServicesLayer.Services.Settings.RabbitMQListener;
using ServicesLayer.Services.Settings.RateLimiting;
using StackExchange.Redis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace AHUB_Test
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			//get secrets from .json file inside project root dir (to solve problem of migration from Win to Linux)
			var root = Directory.GetParent(Directory.GetCurrentDirectory());
			var secretsPath = Path.Combine(root.FullName, "userSecrets.json");
			SecretsManager.Load(secretsPath);

			//allow usage of environment variables
			builder.Configuration.AddEnvironmentVariables();
				//.AddUserSecrets(Assembly.GetExecutingAssembly(), true);

			// configure Redis-based distributed session
			var redisConnection = builder.Environment.IsDevelopment() ?
				builder.Configuration.GetValue<string>("RedisConnection:localAddress")
				: builder.Configuration.GetValue<string>("RedisConnection:dockerAddress");

			var redisConfigurationOptions = ConfigurationOptions.Parse(redisConnection);
			var redis = ConnectionMultiplexer
				.Connect(redisConfigurationOptions);
			builder.Services.AddDataProtection()
				.PersistKeysToStackExchangeRedis(redis, "DataProtectionKeys");

			builder.Services.AddStackExchangeRedisCache(redisCacheConfig =>
			{
				redisCacheConfig.InstanceName = "test_redis_instance";
				redisCacheConfig.ConfigurationOptions = redisConfigurationOptions;
			});

			builder.Services.AddSession(op =>
			{
				op.Cookie.Name = "test_session";
				op.IdleTimeout = TimeSpan.FromMinutes(40);
				
			});

			// if on Linux, add config entry
			//if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			//{
			//	logger.LogInformation("OS is Linux");
			//	builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
			//}

			

			// Add services to the container.

			//add database configuration
			builder.Services.AddDbContext<AHUBContext>(options =>
			{
				var connStr = builder.Environment.IsDevelopment() ?
				builder.Configuration["DbConnectionDev"] : builder.Configuration["DbConnectionProd"];

				options.UseNpgsql(connStr, builder => builder.MigrationsAssembly("DomainLayer"));
			});

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(opt =>
			{
				opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
				opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please, enter a valid JWT token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});

				var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "AHUB_Test.xml");
				opt.IncludeXmlComments(xmlDocPath);

				opt.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[]{ }
					}
					
				});
			});

			// add jwt token authentication to the project
			builder.Services.AddAuthentication(opts =>
			{
				opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(b =>
				{
					b.RequireHttpsMetadata = false;
					b.SaveToken = true;
					b.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
					{
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKey"])),
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateIssuerSigningKey = true,
						ValidateLifetime = false
					};
					b.Events = new JwtBearerEvents
					{
						OnAuthenticationFailed = context =>
						{
							if(context?.Exception?.GetType() 
								== typeof(SecurityTokenExpiredException))
							{
								context?.Response?.Headers?.Add("IS-TOKEN-EXPIRED", "true");
							}
							return Task.CompletedTask;
						}
					};
				});

			#region Services Injected
			builder.Services.AddScoped(typeof(IRepositoryManager), typeof(RepositoryManager));
			builder.Services.AddScoped(typeof(IServiceManager), typeof(ServiceManager));
			#endregion

			builder.Services.AddSingleton<RateLimiterService>();

			// initialize redis multiplexer
			builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfigurationOptions));


			builder.Services.AddHostedService<RabbitMQListener>();

			// initialize elastic connection
			builder.Services.AddElasticsearch(builder.Configuration);

			var app = builder.Build();

			app.UseFixedWindowRateLimiter();

			//enable session for the app
			app.UseSession();

			// Configure the HTTP request pipeline.
			app.UseSwagger();
			app.UseSwaggerUI();


			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();


			app.Run();
		}
	}
}