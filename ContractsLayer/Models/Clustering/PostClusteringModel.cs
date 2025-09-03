

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ContractsLayer.Models.Clustering
{
    /// <summary>
    /// модель публикации пользователя для выполнения сериализации
    /// </summary>
    [XmlType("PostClusteringModel")]
    [Serializable]
    public class PostClusteringModel
    {
        /// <summary>
        /// название поста
        /// </summary>	
        [XmlElement("title")]
        public string title { get; set; }

        /// <summary>
        /// содержимое поста
        /// </summary> 
        [XmlElement("content")]
        public string content { get; set; }

        /// <summary>
        /// имя человека/ название компании, от лица которой опубликован пост
        /// </summary>
        [XmlElement("publisherName")]
        public string? publisherName { get; set; }

        /// <summary>
        /// URL ссылки, отмеченной в посте
        /// </summary>
        [XmlElement("linkURL")]
        public string? linkURL { get; set; }

        /// <summary>
        /// текст ссылки, отображаемый в посте
        /// </summary>
        [XmlElement("linkName")]
        public string? linkName { get; set; }

        /// <summary>
        /// физический адрес, к которму относится метка
        /// </summary>
        [XmlElement("geoTag")]
        public string? geoTag { get; set; }      
        
        /// <summary>
        /// код пользователя
        /// </summary>
        [XmlElement("userId")]
        public Guid userId { get; set; }

        /// <summary>
        /// код поста из внешней системы;
        /// нужен для сопоставления записей в сервисе кластеризации
        /// </summary>
        [XmlElement("outerServiceId")]
        public Guid? outerServiceId { get; set; }

    }
}
