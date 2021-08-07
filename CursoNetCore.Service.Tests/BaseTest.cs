using AutoFixture;
using AutoMapper;
using CursoNetCore.CrossCutting.Mappings;

namespace CursoNetCore.Service.Tests
{
    public abstract class BaseTest
    {
        protected readonly IMapper _mapper;
        protected readonly Fixture _fixture;

        public BaseTest()
        {
            _mapper = GetMapper();
            _fixture = GetFixture();
        }

        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping());
            });
            return config.CreateMapper();
        }

        private Fixture GetFixture()
        {
            var fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
