using AutoMapper;
using Shared.Domain.Tables;
using Shared.ViewModel.Users;

namespace Shared.Mapping
{
    public class DotNetExerciseProfile : Profile
    {
        public DotNetExerciseProfile()
        {
            CreateMap<UserModel, User>().ReverseMap();
        }
    }
}
