using System.ComponentModel.DataAnnotations;
using Violetum.Domain.Infrastructure;

namespace Violetum.Domain.Models.SearchParams
{
    public abstract class BaseSearchParams<TEntity> : ISearchParams<TEntity>
    {
        public string SortBy { get; set; } = "CreatedAt";
        public string OrderByDir { get; set; } = "desc";

        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be lower than {1}.")]
        public int CurrentPage { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "The field {0} must be lower than {1}.")]
        public int Limit { get; set; } = 20;

        public int Offset => Limit * (CurrentPage - 1);
    }
}