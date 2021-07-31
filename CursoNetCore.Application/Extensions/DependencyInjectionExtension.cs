using CursoNetCore.CrossCutting.Mappings;
using CursoNetCore.Data;
using CursoNetCore.Data.Repository;
using CursoNetCore.Domain.Interfaces;
using CursoNetCore.Domain.Interfaces.Services;
using CursoNetCore.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoNetCore.Application.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<Context>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.22"));
                options.EnableSensitiveDataLogging().LogTo(Console.WriteLine);
            });
            return services;
        }

        public static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            return services;
        }

        public static IServiceCollection AddTransientServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITokenService, TokenService>();

            return services;
        }

        public static IServiceCollection AddAuthenticationJwt(this IServiceCollection services, string secretKey)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            return services;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var config = new AutoMapper.MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping());
            });
            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CursoNetCore.Application",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Entre com o token JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference() { Id = "Bearer", Type = ReferenceType.SecurityScheme }},
                        new List<string>()
                    }
                });
            });

            return services;
        }
    }
}
