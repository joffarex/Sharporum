using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Joffarex.Specification
{
    public class EfSpecificationEvaluator<T, TResult> where T : class where TResult : class
    {
        public static IQueryable<TResult> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification,
            IConfigurationProvider configurationProvider)
        {
            var query = inputQuery;

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));

            var projectedQuery = SpecificationEvaluator<T, TResult>
                .GetQuery(query, specification, configurationProvider);

            return projectedQuery;
        }
    }

    public class EfSpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));

            return SpecificationEvaluator<T>.GetQuery(query, specification);
        }
    }
}