using ContractsLayer.Base;
using System.ComponentModel.DataAnnotations;

namespace ContractsLayer.Dtos
{
    /// <summary>
    /// модель представления "пользователь"
    /// </summary>
	public class UserDto: BaseEntityModel
	{
        /// <summary>
        /// логин
        /// </summary>
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string userName { get; set; }

		/// <summary>
		/// пароль
		/// </summary>
		[Required]
		[MinLength(7)]
		[MaxLength(25)]
		public string password { get; set; }

		/// <summary>
		/// имя
		/// </summary>
		[Required]
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

        /// <summary>
        /// флаг - запись удалена
        /// </summary>
        public bool isRemoved { get; set; } = false;
    }
}
