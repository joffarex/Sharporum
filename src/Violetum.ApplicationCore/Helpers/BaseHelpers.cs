using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Violetum.ApplicationCore.Helpers
{
    public static class BaseHelpers
    {
        public static Func<T, object> GetOrderByExpression<T>(string sortColumn)
        {
            Func<T, object> orderByExpr = null;
            if (!string.IsNullOrEmpty(sortColumn))
            {
                Type sponsorResultType = typeof(T);

                if (sponsorResultType.GetProperties().Any(prop => prop.Name == sortColumn))
                {
                    PropertyInfo pinfo = sponsorResultType.GetProperty(sortColumn);
                    orderByExpr = data => pinfo.GetValue(data, null);
                }
            }

            return orderByExpr;
        }

        public static async Task<string> UploadImageToBucketAndGetUrl(string image)
        {
            throw new NotImplementedException();
        }
    }
}