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
    internal class GrammarBlockMapper : IInfrastructureMapper<GrammarBlock, GrammarBlockModel>
    {
        public GrammarBlock MapToDomain(GrammarBlockModel infrastucture)
        {
            return new GrammarBlock
            {
                RuleId = infrastucture.RuleId,
                Text = infrastucture.Text,
                Type = infrastucture.Type,
                Ch = infrastucture.Ch,
                Pn = infrastucture.Pn,
                Rus = infrastucture.Rus
            };
        }

        public GrammarBlockModel MapToModel(GrammarBlock domain)
        {
            return new GrammarBlockModel
            {
                RuleId = domain.RuleId,
                Text = domain.Text,
                Type = domain.Type,
                Ch = domain.Ch,
                Pn = domain.Pn,
                Rus = domain.Rus,
                
            };
        }
    }
}
