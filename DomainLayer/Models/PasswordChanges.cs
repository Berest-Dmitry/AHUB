using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность, хранящая инф-ю
    /// о смене паролей пользователями
    /// </summary>
    public class PasswordChanges : EntityBase
    {
        /// <summary>
        /// код пользователя, меняющего пароль
        /// </summary>
        public Guid userId { get; set; }

        public User user { get; set; }

        /// <summary>
        /// код для смены пароля
        /// </summary>
        public int recoveryKey { get; set; }

        /// <summary>
        /// защитный токен для смены пароля
        /// </summary>
        public string recoveryToken { get; set; }

        /// <summary>
        /// дата и время, до которой действителен код смены пароля
        /// </summary>
        public DateTime? keyValidBefore { get; set; }

        /// <summary>
        /// дата и время, до которой пользователь может поменять пароль
        /// после подтверждения кода доступа
        /// </summary>
        public DateTime? changePasswordBefore { get; set; }

        /// <summary>
        /// дата и время замены пароля
        /// </summary>
        public DateTime? passwordChangeTime { get; set; }
    }
}
