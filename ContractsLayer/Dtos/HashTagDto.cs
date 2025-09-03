using ContractsLayer.Base;


namespace ContractsLayer.Dtos
{
	/// <summary>
	/// модель представления хештега
	/// </summary>
	public class HashTagDto: BaseEntityModel
	{
		/// <summary>
		/// содержимое
		/// </summary>
		public string content {  get; set; }

		/// <summary>
		/// флаг - помечен удаленным
		/// </summary>
		public bool isRemoved { get; set; } = false;
	}
}
