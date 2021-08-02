using AutoMapper;
using CursoNetCore.CrossCutting.Mappings;

namespace CursoNetCore.Service.Tests
{
    public abstract class BaseTest
    {
        protected readonly IMapper _mapper;

        public BaseTest()
        {
            _mapper = GetMapper();
        }

        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping());
            });
            return config.CreateMapper();
        }
    }
}
