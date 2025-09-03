namespace DomainLayer.Models.Base
{
    public class EntityBase
    {
        /// <summary>
        /// ключ записи
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// время добавления
        /// </summary>
        public DateTime dateTimeAdded { get; set; }

        /// <summary>
        /// время удаления
        /// </summary>
        public DateTime? deletedAt { get; set; }

        public EntityBase()
        {
            id = Guid.NewGuid();
            dateTimeAdded = DateTime.Now.ToUniversalTime();
        }
    }
}
