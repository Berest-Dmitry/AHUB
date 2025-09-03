using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность заявления о нарушении
    /// </summary>
    public class Appeal : EntityBase
    {
        /// <summary>
        /// причина жалобы
        /// </summary>
        public short reason { get; set; }

        /// <summary>
        /// статус рассмотрения
        /// </summary>
        public short status { get; set; } = 0;

        /// <summary>
        /// комментарий  пользователя
        /// </summary>
        public string? comment { get; set; }

        /// <summary>
        /// код пользователя
        /// </summary>
        public Guid userId { get; set; }

        public User User { get; set; }

        /// <summary>
        /// код сущности, на которую выполнена жалоба
        /// </summary>
        public Guid appealEntityId { get; set; }
    }
}
