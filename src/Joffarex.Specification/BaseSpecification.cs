#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Joffarex.Specification.Extensions.Include;

namespace Joffarex.Specification
{
    public abstract class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria) : base(criteria)
        {
        }

        public Expression<Func<T, TResult>>? Selector { get; set; }
    }

    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification()
        {
        }

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            AddCriteria(criteria);
        }

        public IEnumerable<Expression<Func<T, bool>>> Criterias { get; } = new List<Expression<Func<T, bool>>>();
        public IEnumerable<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public IEnumerable<string> IncludeStrings { get; } = new List<string>();

        public Expression<Func<T, object>>? ThenBy { get; private set; }
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public string? FieldOrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public string? FieldOrderByDescending { get; private set; }
        //public Expression<Func<T, object>> GroupBy { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;

        public string? CacheKey { get; protected set; }
        public bool CacheEnabled { get; private set; }

        protected virtual void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            ((List<Expression<Func<T, bool>>>) Criterias).Add(criteria);
        }

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            ((List<Expression<Func<T, object>>>) Includes).Add(includeExpression);
        }

        protected virtual void AddIncludes<TProperty>(
            Func<IncludeAggregator<T>, IIncludeQuery<T, TProperty>> includeGenerator)
        {
            var includeQuery = includeGenerator(new IncludeAggregator<T>());
            ((List<string>) IncludeStrings).AddRange(includeQuery.Paths);
        }

        protected virtual void AddInclude(string includeString)
        {
            ((List<string>) IncludeStrings).Add(includeString);
        }

        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyFieldOrderBy(string fieldName)
        {
            FieldOrderBy = fieldName;
        }

        protected virtual void ApplyOrderByWithThenBy(Expression<Func<T, object>> orderByExpression,
            Expression<Func<T, object>> thenByExpression)
        {
            OrderBy = orderByExpression;
            ThenBy = thenByExpression;
        }

        protected virtual void ApplyOrderByDescendingWithThenBy(Expression<Func<T, object>> orderByDescendingExpression,
            Expression<Func<T, object>> thenByExpression)
        {
            OrderBy = orderByDescendingExpression;
            ThenBy = thenByExpression;
        }

        protected virtual void ApplyFieldOrderByDescending(string fieldName)
        {
            FieldOrderByDescending = fieldName;
        }
    }
}