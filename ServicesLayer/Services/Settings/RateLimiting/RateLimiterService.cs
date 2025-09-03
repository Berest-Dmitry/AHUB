using ContractsLayer.Base;
using Microsoft.Extensions.Configuration;
using ServicesLayer.IServices;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services.Settings.RateLimiting
{
    public class RateLimiterService
    {
        private readonly IDatabase _db;

        private const string RATE_LIMITER = @"
            local requests = redis.call('INCR',@key)
            redis.call('EXPIRE', @key, @expiry)
            if requests < tonumber(@maxRequests) then
                return 0
            else
                return 1
            end
            ";

        public static LuaScript RateLimitScript => LuaScript.Prepare(RATE_LIMITER);

        public RateLimiterService(IConnectionMultiplexer mux)
        {
            _db = mux.GetDatabase();
        }

        /// <summary>
        /// метод ограничения кол-ва запросов к эндпоинту
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public async Task<BaseModel> LimitNumberOfRequests(string requestPath, string apiKey, int expiry, int maxRequests)
        {
            try
            {
                var key = $"{requestPath}:{apiKey}:{DateTime.UtcNow:hh:mm}";
                var result = await _db.ScriptEvaluateAsync(RateLimitScript,
                    new
                    {
                        key = new RedisKey(key),
                        expiry,
                        maxRequests
                    });

                if ((int)result == 1)
                {
                    return new BaseModel(new Exception("Too many requests sent!"));
                }
                else
                {
                    return new BaseModel();
                }
            }
            catch (Exception ex)
            {
                return new BaseModel(ex);
            }
        }
    }
}
