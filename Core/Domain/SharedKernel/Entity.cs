using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.SharedKernel
{
    public abstract class Entity<T>
    {
        protected Entity(T id)
        {
            Id = id;
        }

        public T Id { get; }
    }
}
