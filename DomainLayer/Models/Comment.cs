using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность комментария к посту
    /// </summary>
    public class Comment : EntityBase
    {
        /// <summary>
        /// текст комментария
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// код пользователя, оставившего комментарий
        /// </summary>
        public Guid userId { get; set; }

        public User User { get; set; }

        /// <summary>
        /// код поста, к которому оставлен комментарий
        /// </summary>
        public Guid postId { get; set; }

        public Post Post { get; set; }

        /// <summary>
        /// код родительского комментария
        /// </summary>
        public Guid? parentId { get; set; }

        /// <summary>
        /// дата-время редактирования комментария
        /// </summary>
        public DateTime? dateTimeEdited { get; set; }
    }
}
