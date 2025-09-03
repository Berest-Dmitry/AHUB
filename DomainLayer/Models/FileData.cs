using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность, хранящая инф-ю о файлах в системе
    /// </summary>
    public class FileData : EntityBase
    {
        /// <summary>
        /// название файла
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// путь к файлу
        /// </summary>
        public string? filePath { get; set; }

        /// <summary>
        /// тип содержимого файла
        /// </summary>
        public string mediaType { get; set; }

        /// <summary>
        /// размер файла
        /// </summary>
        public string? fileSize { get; set; }

        /// <summary>
        /// название bucket'а, в котором хранится данный файл в облачном хранилище
        /// </summary>
        public string bucketName { get; set; }

        /// <summary>
        /// код сущности-владельца файла
        /// </summary>
        public Guid? entityOwnerId { get; set; }
    }
}
