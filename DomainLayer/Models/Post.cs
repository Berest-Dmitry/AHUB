using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// запись в БД о публикации пользователя (посте)
    /// </summary>
    public class Post : EntityBase
    {
        /// <summary>
        /// название поста
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// содержимое поста
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// имя человека/ название компании, от лица которой опубликован пост
        /// </summary>
        public string? publisherName { get; set; }

        /// <summary>
        /// URL ссылки, отмеченной в посте
        /// </summary>
        public string? linkURL { get; set; }

        /// <summary>
        /// текст ссылки, отображаемый в посте
        /// </summary>
        public string? linkName { get; set; }

        /// <summary>
        /// гео-метка места, упомянутого в посте
        /// </summary>
        public string? geoTag { get; set; }

        /// <summary>
        /// код пользователя-публикатора
        /// </summary>
        public Guid userId { get; set; }

        public User User { get; set; }

        public List<PostFile> PostFiles { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
