using ContractsLayer.Base;


namespace ContractsLayer.Dtos
{
	/// <summary>
	/// модель представления роли пользователя
	/// </summary>
	public class UserRoleDto: BaseEntityModel
	{
		/// <summary>
		/// код роли
		/// </summary>
		public Guid roleId { get; set; }

		/// <summary>
		/// код пользователя
		/// </summary>
		public Guid userId { get; set; }
	}
}
