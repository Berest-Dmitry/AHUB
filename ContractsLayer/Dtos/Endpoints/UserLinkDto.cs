using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель пользователя для работы ссылок (упоминаний)
	/// </summary>
	public class UserLinkDto
	{
		public Guid userId {  get; set; }

		public string fullName { get; set; }
	}
}
