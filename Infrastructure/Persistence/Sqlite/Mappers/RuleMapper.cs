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
    internal class RuleMapper : IInfrastructureMapper<Rule, RuleModel>
    {
        public Rule MapToDomain(RuleModel infrastucture)
        {
            return new Rule
            {
                Id = infrastucture.RuleId,
                Title = infrastucture.Title
            };
        }

        public RuleModel MapToModel(Rule domain)
        {
            return new RuleModel
            {
                RuleId = domain.Id,
                Title = domain.Title,
            };
        }
    }
}
