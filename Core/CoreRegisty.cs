using Core.Domain.Abstractions.Sevices;
using Core.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class CoreRegisty
    {
        public static IServiceCollection RegistryCoreServices(this IServiceCollection services)
        {
            return services
                .AddTransient<ILessonService, LessonService>()
                .AddTransient<INotebookService, NotebookService>()
                .AddTransient<IWordService, WordService>()
                .AddTransient<IRuleService, RuleService>();
        }
    }
}
