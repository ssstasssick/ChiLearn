using Core.Domain.Entity;
using Core.Persistence;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Configuration;
using Infrastructure.Persistence.Sqlite.Mappers;
using Infrastructure.Persistence.Sqlite.Models;
using Infrastructure.Persistence.Sqlite.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ChiLearn.Infrastructure
{
    public static class InfrastructureRegistry
    {
        public static IServiceCollection RegisterInfrastuctureService(this IServiceCollection services, InfrastuctureConfiguration configuration)
        {
            return services
                .RegisterPersistence(configuration)
                .RegisterMappers()
                .RegisterRepositories();
        }

        private static IServiceCollection RegisterPersistence(this IServiceCollection services, InfrastuctureConfiguration configuration)
        {
            return services
                .AddSingleton(configuration)
                .AddTransient<DbConnection>();
        }

        private static IServiceCollection RegisterMappers(this IServiceCollection services)
        {
            return services
                .AddTransient<IInfrastructureMapper<Word, WordModel>, WordMapper>()
                .AddTransient<IInfrastructureMapper<Lesson, LessonModel>, LessonMapper>()
                .AddTransient<IInfrastructureMapper<LessonWord, LessonWordModel>, LessonWordMapper>();
        }

        private static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            return services
                .AddTransient<IWordRepository, WordRepository>()
                .AddTransient<ILessonRepository, LessonRepository>()
                .AddTransient<ILessonWordRepository, LessonWordRepository>();
        }
    }
}

