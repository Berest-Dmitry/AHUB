using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность-связка между пользователем и ролью
    /// </summary>
    public class UserRoles : EntityBase
    {
        /// <summary>
        /// код пользователя
        /// </summary>
        public Guid userId { get; set; }

        public User User { get; set; }

        /// <summary>
        /// код роли
        /// </summary>
        public Guid roleId { get; set; }

        public Role Role { get; set; }

        public UserRoles(Guid userId, Guid roleId) : base()
        { 
            this.userId = userId;
            this.roleId = roleId;
        }
    }
}
