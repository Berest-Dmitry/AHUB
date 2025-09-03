using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность "пользователь"
    /// </summary>
    public class User : EntityBase
    {
        /// <summary>
        /// логин
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// пароль в хешированном виде
        /// </summary>
        public string hashedPassword { get; set; }

        /// <summary>
        /// соль хеша
        /// </summary>
        public string salt { get; set; }

        /// <summary>
        /// имя
        /// </summary>
        public string firstName { get; set; }

        /// <summary>
        /// фамилия
        /// </summary>
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

        public List<PasswordChanges> PasswordChanges { get; set; }

        public List<Post> Posts { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Appeal> Appeals { get; set; }

        public List<UserRoles> Roles { get; set; }
    }
}
