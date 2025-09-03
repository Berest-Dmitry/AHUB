using ContractsLayer.Base;


namespace ContractsLayer.Dtos
{
	/// <summary>
	/// модель представления роли в системе
	/// </summary>
	public class RoleDto: BaseEntityModel
	{
		/// <summary>
		/// название роли
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// флаг - роль удалена
		/// </summary>
		public bool isRemoved { get; set; } = false;
	}
}
