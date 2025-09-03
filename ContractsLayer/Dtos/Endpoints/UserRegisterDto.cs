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
    /// модель регистрации пользователя
    /// </summary>
    public class UserRegisterDto : BaseEntityModel
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
    }
}
