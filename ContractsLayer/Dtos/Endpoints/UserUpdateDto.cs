using ContractsLayer.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель представления для обновления данных пользователя
	/// </summary>
	public class UserUpdateDto: BaseEntityModel
	{
		/// <summary>
		/// имя
		/// </summary>
		[MinLength(3)]
		[MaxLength(50)]
		public string firstName { get; set; }

		/// <summary>
		/// фамилия
		/// </summary>
		[Required]
		[MinLength(2)]
		[MaxLength(50)]
		public string lastName { get; set; }

		/// <summary>
		/// дата рождения
		/// </summary>
		public DateTime? birthday { get; set; }

		/// <summary>
		/// пол
		/// </summary>
		public short? gender { get; set; }

		/// <summary>
		/// инф-я об образовании
		/// </summary>
		public string? educationInfo { get; set; }

		/// <summary>
		/// инф-я о месте работы
		/// </summary>
		public string? jobInfo { get; set; }

		/// <summary>
		/// место жительства
		/// </summary>
		public string? homeTown { get; set; }
	}
}
