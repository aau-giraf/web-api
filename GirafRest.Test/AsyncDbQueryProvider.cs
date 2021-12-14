using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using System;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace GirafRest.Test 
{ 
    internal class TestDbAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    { 
        private readonly IQueryProvider _inner; 
 
        internal TestDbAsyncQueryProvider(IQueryProvider inner) 
        { 
            _inner = inner;
            if (inner == null)
                throw new ArgumentNullException("The QueryProvider may not be null.");
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
            if (expression == null)
                throw new ArgumentNullException("The expression is null!");
            if (_inner == null)
                throw new ArgumentNullException("Expression evaluator is null!");
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

        // Appeared from the update to .NET Core 3.1.
        // Reference: https://stackoverflow.com/questions/57314896/iasyncqueryprovider-mock-issue-when-migrated-to-net-core-3-adding-tresult-iasyn
        // That leads to -> https://github.com/romantitov/MockQueryable/blob/master/src/MockQueryable/MockQueryable/TestAsyncEnumerable.cs
        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                                 .GetMethod(
                                      name: nameof(IQueryProvider.Execute),
                                      genericParameterCount: 1,
                                      types: new[] { typeof(Expression) })
                                 .MakeGenericMethod(expectedResultType)
                                 .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                                        ?.MakeGenericMethod(expectedResultType)
                                         .Invoke(null, new[] { executionResult });
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

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
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

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            // Attempt to dispose, throw on failure
            try
            {
                _inner.Dispose();
                return default;
            }
            catch (Exception exception)
            {
                return new ValueTask(Task.FromException(exception));
            }
        }

        T IAsyncEnumerator<T>.Current => _inner.Current;
    } 
}