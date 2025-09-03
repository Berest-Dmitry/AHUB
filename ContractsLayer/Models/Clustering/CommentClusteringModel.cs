using System.Xml.Serialization;

namespace ContractsLayer.Models.Clustering
{
    /// <summary>
    /// модель комментариев пользователей для выполнения кластеризации
    /// </summary>
    [XmlType("CommentClusteringModel")]
    [Serializable]
    public class CommentClusteringModel
    {
        /// <summary>
        /// текст комментария
        /// </summary>	
        [XmlElement("content")]
        public string content { get; set; }

        /// <summary>
        /// код пользователя, опубликовавшего пост
        /// </summary>
        [XmlElement("userId")]
        public Guid userId { get; set; }

        /// <summary>
        /// код родительского поста
        /// </summary>
        [XmlElement("postId")]
        public Guid postId { get; set; }

        /// <summary>
        /// код родительского комментария
        /// </summary>
        [XmlElement("parentId")]
        public Guid? parentId { get; set; }
    }
}
