using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace GirafRest.Test.Mocks
{
    class MockEntityEntry<T> : EntityEntry<T>
        where T : class
    {
        private T overriddenEntity;

        public override T Entity
        {
            get
            {
                return overriddenEntity;
            }
        }
        public MockEntityEntry(T result) : base(null)
        {
            this.overriddenEntity = result;
        }
    }
}
