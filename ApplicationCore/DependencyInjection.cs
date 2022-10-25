using System;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Infrastructure.Persistence;

namespace ApplicationCore
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationCore(this IServiceCollection services)
		{
            services.AddControllers(options =>
                options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddFluentValidation();
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddSqlite<MyAppDbContext>(connectionString);
            return services;
        }
    }
}

