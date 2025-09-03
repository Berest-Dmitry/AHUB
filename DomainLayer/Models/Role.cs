using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность роли пользователя
    /// </summary>
    public class Role : EntityBase
    {
        /// <summary>
        /// название роли
        /// </summary>
        public string name { get; set; }

        public List<UserRoles> Users { get; set; }

        public Role() : base()
        { }

        public Role(string name)
            : base()
        {
            this.name = name;
        }
    }
}
