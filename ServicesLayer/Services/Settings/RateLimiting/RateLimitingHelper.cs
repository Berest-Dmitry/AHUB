using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace ServicesLayer.Services.Settings.RateLimiting
{
	/// <summary>
	/// вспомогательный класс для использования в алгоритме ограничения по времени
	/// </summary>
	public static class RateLimitingHelper
	{
		/// <summary>
		/// получение логина пользователя, который осуществляет запрос
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string GetApiKey(HttpContext context)
		{
			return context?.Request?.Headers["userName"];
		}

		/// <summary>
		/// метод сравнения текущего пути с путем, указанным в настройках
		/// </summary>
		/// <param name="currentPath"></param>
		/// <param name="Path"></param>
		/// <param name="PathRegex"></param>
		/// <returns></returns>
		public static bool MatchPath(string currentPath, string Path, string PathRegex)
		{
			if(!string.IsNullOrEmpty(Path))
			{
				return currentPath.Equals(Path, StringComparison.InvariantCultureIgnoreCase);
			}
			if(!string.IsNullOrEmpty(PathRegex))
			{
				return Regex.IsMatch(currentPath, PathRegex);
			}
			return false;
		}
	}
}
