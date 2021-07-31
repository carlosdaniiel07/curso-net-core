using AutoMapper;
using CursoNetCore.Domain.Dtos.User;
using CursoNetCore.Domain.Entities;

namespace CursoNetCore.CrossCutting.Mappings
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            #region User
            
            CreateMap<SaveUserDto, User>();
            CreateMap<User, UserDto>();
            
            #endregion
        }
    }
}
