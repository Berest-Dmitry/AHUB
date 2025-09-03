using ContractsLayer.Common;
using ContractsLayer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServicesLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services.Settings.RateLimiting
{
	/// <summary>
	/// Класс для обработки запросов на ограничение доступа к приложению по времени 
	/// </summary>
	public class RateLimitingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IConfiguration _configuration;
		private readonly ILogger<RateLimitingMiddleware> _logger;
		private readonly RateLimiterService _limiterService;
		
		public RateLimitingMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RateLimitingMiddleware> logger, RateLimiterService rateLimiterService)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_limiterService = rateLimiterService ?? throw new ArgumentNullException(nameof(rateLimiterService));
		}

		/// <summary>
		/// метод получения настроек ограничителя по времени
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private RateLimitRule GetApplicableRules(HttpContext context)
		{
			var rules = _configuration.GetSection("RateLimiterSettings").Get<RateLimitRule[]>();
			var applicableRule = rules
				.Where(r => RateLimitingHelper.MatchPath(context?.Request?.Path, r.Path, r.PathRegex))
				.OrderBy(r => r.maxRequests)
				.GroupBy(r => new { r.PathKey, r.expiry })
				.Select(r => r.First());
			return applicableRule.FirstOrDefault();
		}

		/// <summary>
		/// метод вызова алгоритма
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				var currentRule = GetApplicableRules(httpContext);
				var apiKey = RateLimitingHelper.GetApiKey(httpContext);

				if(currentRule == null)
				{
					await _next(httpContext);
				}

				var result = await _limiterService
					.LimitNumberOfRequests(httpContext?.Request?.Path
					, apiKey
					, Convert.ToInt16(currentRule?.expiry)
					, Convert.ToInt16(currentRule?.maxRequests));

				var limited = result.Result == DefaultEnums.Result.error;
				if(limited)
				{
					httpContext.Response.StatusCode = 429;
					return;
				}
				await _next(httpContext);
			}
			catch(Exception ex)
			{
				_logger.LogWarning("An error occurred in rate limiting middleware: " + ex.Message);
			}
		}
	}

	/// <summary>
	/// класс расширения приложения для использования ограничителя по времени
	/// </summary>
	public static class RateLimiterExtensions
	{
		public static void UseFixedWindowRateLimiter(this IApplicationBuilder builder)
		{
			builder.UseMiddleware<RateLimitingMiddleware>();
		}
	}
}
