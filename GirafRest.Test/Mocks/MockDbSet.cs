using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace GirafRest.Test.Mocks
{
    public class MockDbSet<T> : DbSet<T>
        where T : class
    {
        private List<T> entities;

        public MockDbSet(List<T> sampleEntries)
        {
            entities = new List<T>();
            entities.AddRange(sampleEntries);
        }

        public MockDbSet()
        {
            entities = new List<T>();
        }

        public override Task<EntityEntry<T>> AddAsync(T entity, CancellationToken ct = default(CancellationToken))
        {
            entities.Add(entity);
            return Task.FromResult<EntityEntry<T>>(new MockEntityEntry<T>(entity));
        }

        public override EntityEntry<T> Add(T entity)
        {
            entities.Add(entity);
            return new MockEntityEntry<T>(entity);
        }

        public override EntityEntry<T> Remove(T entity)
        {
            entities.Remove(entity);
            return new MockEntityEntry<T>(entity);
        }
    }
}
