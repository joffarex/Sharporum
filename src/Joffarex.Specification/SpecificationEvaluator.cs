using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Joffarex.Specification.Extensions;

namespace Joffarex.Specification
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criterias.Count() > 0)
            {
                query = specification.Criterias.Aggregate(query,
                    (current, criteria) => current.Where(criteria));
            }

            // Apply ordering if expressions are set
            if (specification.FieldOrderBy != null)
            {
                query = query.OrderBy(specification.FieldOrderBy);
            }
            else if (specification.FieldOrderByDescending != null)
            {
                query = query.OrderByDescending(specification.FieldOrderByDescending);
            }

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                    .Take(specification.Take);
            }

            return query;
        }
    }

    public class SpecificationEvaluator<T, TResult> where T : class where TResult : class
    {
        public static IQueryable<TResult> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification,
            IConfigurationProvider configurationProvider)
        {
            var query = inputQuery;

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criterias.Count() > 0)
            {
                query = specification.Criterias.Aggregate(query,
                    (current, criteria) => current.Where(criteria));
            }

            var projectedQuery = query.ProjectTo<TResult>(configurationProvider);

            // Apply ordering if expressions are set
            if (specification.FieldOrderBy != null)
            {
                projectedQuery = projectedQuery.OrderBy(specification.FieldOrderBy);
            }
            else if (specification.FieldOrderByDescending != null)
            {
                projectedQuery = projectedQuery.OrderByDescending(specification.FieldOrderByDescending);
            }

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                projectedQuery = projectedQuery.Skip(specification.Skip)
                    .Take(specification.Take);
            }

            return projectedQuery;
        }
    }
}