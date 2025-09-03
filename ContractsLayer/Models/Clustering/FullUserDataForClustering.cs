using System.Xml.Serialization;

namespace ContractsLayer.Models.Clustering
{
    /// <summary>
    /// модель, содержащая набор данных по пользователю,
    /// для выполнения кластеризации
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(CommentClusteringModel))]
    [XmlInclude(typeof(PostClusteringModel))]
    public class FullUserDataForClustering
    {
        /// <summary>
        /// имя
        /// </summary>
        [XmlElement("firstName")]
        public string firstName { get; set; } = string.Empty;

        /// <summary>
        /// фамилия
        /// </summary>
        [XmlElement("lastName")]
        public string lastName { get; set; } = string.Empty;

        /// <summary>
        /// дата рождения
        /// </summary>
        [XmlElement("birthday")]
        public DateTime birthday { get; set; }
        /// <summary>
        /// пол
        /// </summary>
        [XmlElement("gender")]
        public short? gender { get; set; }

        /// <summary>
        /// инф-я об образовании
        /// </summary>
        [XmlElement("educationInfo")]
        public string? educationInfo { get; set; } = string.Empty;

        /// <summary>
        /// дата регистрации на платформе
        /// </summary>
        [XmlElement("registrationDate")]
        public DateTime registrationDate { get; set; }
        
        /// <summary>
        /// код сущности из внешней системы;
        /// нужен для сопоставления записей в сервисе кластеризации
        /// </summary>
        public Guid outerServiceId { get; set; }
        
        /// <summary>
        /// посты, опубликованные текущим пользователем
        /// </summary>
        [XmlArray("userPosts")]
        [XmlArrayItem("post")]
        public List<PostClusteringModel> userPosts { get; set; } = new();

        /// <summary>
        /// комментарии, написанные текущим пользователем
        /// </summary>
        [XmlArray("userComments")]
        [XmlArrayItem("comment")]
        public List<CommentClusteringModel> userComments { get; set; } = new();
    }
}
