using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Abstractions.Internal
{
    internal interface IInfrastructureMapper<TDomain, TInfrastucture>
    {
        public TDomain MapToDomain(TInfrastucture infrastucture);
        public TInfrastucture MapToModel(TDomain domain);

    }
}
