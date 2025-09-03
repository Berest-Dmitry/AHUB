using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность, хранящая инф-ю о файле, прикрепленном к посту
    /// </summary>
    public class PostFile : EntityBase
    {
        /// <summary>
        /// код поста, к которому относится файл
        /// </summary>
        public Guid postId { get; set; }

        public Post Post { get; set; }

        /// <summary>
        /// код файла
        /// </summary>
        public Guid fileId { get; set; }

        public PostFile(Guid postId, Guid fileId)
            : base()
        {
            this.postId = postId;
            this.fileId = fileId;
        }
    }
}
