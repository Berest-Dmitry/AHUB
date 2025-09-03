using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Models
{
	/// <summary>
	/// класс модели, хранящей правила ограничения по времени
	/// </summary>
	public class RateLimitRule
	{
		public string Path {  get; set; }
		
		public string PathRegex { get; set; }

		public string expiry {  get; set; }

		public string maxRequests { get; set; }

		public string PathKey => !string.IsNullOrEmpty(Path) ? Path : PathRegex;
	}
}
