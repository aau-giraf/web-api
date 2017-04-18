using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace GirafRest.Test 
{ 
    internal class TestDbAsyncQueryProvider<TEntity> : IAsyncQueryProvider 
    { 
        private readonly IQueryProvider _inner; 
 
        internal TestDbAsyncQueryProvider(IQueryProvider inner) 
        { 
            _inner = inner; 
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression); 
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression); 
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            Task<TResult> task = Task.FromResult(Execute<TResult>(expression));
            return (IAsyncEnumerable<TResult>) task;
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            Task<TResult> task = Task.FromResult(Execute<TResult>(expression));
            return task;
        }
    } 
 
    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T> 
    { 
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable) 
            : base(enumerable) 
        { } 
 
        public TestDbAsyncEnumerable(Expression expression) 
            : base(expression) 
        { } 
 
        public IAsyncEnumerator<T> GetAsyncEnumerator() 
        { 
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator()) as IAsyncEnumerator<T>; 
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator()) as IAsyncEnumerator<T>;
        }

        IQueryProvider IQueryable.Provider 
        { 
            get { return new TestDbAsyncQueryProvider<T>(this); } 
        } 
    } 
 
    internal class TestDbAsyncEnumerator<T> : IAsyncEnumerator<T> 
    { 
        private readonly IEnumerator<T> _inner; 
 
        public TestDbAsyncEnumerator(IEnumerator<T> inner) 
        { 
            _inner = inner; 
        } 

        Task<bool> IAsyncEnumerator<T>.MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext()); 
        }

        void IDisposable.Dispose()
        {
            _inner.Dispose(); 
        }

        T IAsyncEnumerator<T>.Current => _inner.Current;
    } 
}