using AutoMapper;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Elastic;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Models.Clustering;
using DomainLayer.Models;
using static ContractsLayer.Common.DefaultEnums;

namespace ServicesLayer.Services
{
	/// <summary>
	/// класс связи моделей и сущностей БД
	/// </summary>
	public static class ObjectMapper
	{
		private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
		{
			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
				cfg.AddProfile<AspnetRunAutoMapper>();
			});
			var mapper = mapperConfig.CreateMapper();
			return mapper;
		});

		public static IMapper Mapper => Lazy.Value;
	}

	public class AspnetRunAutoMapper: Profile
	{
		public AspnetRunAutoMapper()
		{
			CreateMap<User, UserDto>()
				.ForMember(u => u.isRemoved, 
					opt => opt.MapFrom(src => src.deletedAt != null))
				.ReverseMap();

			CreateMap<PasswordChanges, PasswordChangesDto>().ReverseMap();
			CreateMap<UserUpdateDto, User>().ReverseMap();

			CreateMap<FileData, FileDataDto>().ReverseMap();

			CreateMap<Post, PostDto>()
				.ForMember(p => p.geoTag,
					opt => opt.MapFrom(src => src.geoTag))
				.ForMember(p => p.isRemoved,
					opt => opt.MapFrom(src => src.deletedAt != null))
				.ReverseMap();

			CreateMap<PostCreateDto, Post>()
				.ReverseMap();

			CreateMap<PostUpdateDto, Post>()
				.ReverseMap();

			CreateMap<PostFile, PostFileDto>().ReverseMap();

			CreateMap<HashTag, HashTagDto>()
				.ForMember(h => h.isRemoved,
					opt => opt.MapFrom(src => src.deletedAt != null))
				.ReverseMap();

			CreateMap<Comment, CommentDto>()
				.ForMember(c => c.isRemoved,
					opt => opt.MapFrom(src => src.deletedAt != null))
				.ReverseMap();

			CreateMap<CommentCreateDto, Comment>()
				.ReverseMap();

			CreateMap<Appeal, AppealDto>()
				.ForMember(a => a.reason,
					opt => opt.MapFrom(src => (AppealReason)Enum.Parse(typeof(AppealReason), src.reason.ToString())))
				.ForMember(a => a.status,
					opt => opt.MapFrom(src => (AppealStatus)Enum.Parse(typeof(AppealStatus), src.status.ToString())));

			CreateMap<AppealDto, Appeal>()
				.ForMember(a => a.reason,
					opt => opt.MapFrom(src => (int)src.reason))
				.ForMember(a => a.status,
					opt => opt.MapFrom(src => (int)src.status));

			CreateMap<Role, RoleDto>()
				.ForMember(r => r.isRemoved,
					opt => opt.MapFrom(src => src.deletedAt != null))
				.ReverseMap();

			CreateMap<UserRoles, UserRoleDto>()
				.ReverseMap();

			CreateMap<PostDto, PostElDto>()
				.ReverseMap();

			CreateMap<Post, PostElDto>()
				.ReverseMap();

            #region модели_для_кластеризации
			CreateMap<Post, PostClusteringModel>()
				.ForMember(p => p.outerServiceId,
				opt => opt.MapFrom(src => src.id))
				.ReverseMap();

			CreateMap<Comment, CommentClusteringModel>()
				.ReverseMap();
            #endregion

        }
    }
}
