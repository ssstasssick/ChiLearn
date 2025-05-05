using Core.Domain.Entity;
using Infrastructure.Persistence.Abstractions.Internal;
using Infrastructure.Persistence.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Sqlite.Mappers
{
    internal class LessonWordMapper : IInfrastructureMapper<LessonWord, LessonWordModel>
    {
        public LessonWord MapToDomain(LessonWordModel infrastucture)
        {
            return new LessonWord
            {
                LessonWordId = infrastucture.LessonWordId,
                LessonId = infrastucture.LessonId,
                WordId = infrastucture.WordId
            };
        }

        public LessonWordModel MapToModel(LessonWord domain)
        {
            return new LessonWordModel
            {
                LessonWordId = domain.LessonWordId,
                LessonId = domain.LessonId,
                WordId = domain.WordId
            };
        }
    }
}
