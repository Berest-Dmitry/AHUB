using DomainLayer.Models.Base;

namespace DomainLayer.Models
{
    /// <summary>
    /// сущность, хранящая инф-ю о хештегах
    /// </summary>
    public class HashTag : EntityBase
    {
        /// <summary>
        /// содержимое
        /// </summary>
        public string content { get; set; }

        public HashTag(string content)
            : base()
        {
            this.content = content;
        }
    }
}
