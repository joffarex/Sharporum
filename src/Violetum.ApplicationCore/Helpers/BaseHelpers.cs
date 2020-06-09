using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Helpers
{
    public static class BaseHelpers
    {
        private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();

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

        public static FileData GetFileData<TEntity>(string image, string fileName)
        {
            // TODO: add validation
            string[] imageParts = image.Split(",");
            string contentType = imageParts[0].Split("/")[1].Split(";")[0];
            string blobName = $"{typeof(TEntity).Name}/{fileName}.{contentType}";
            return new FileData
            {
                Content = imageParts[1],
                FileName = blobName,
            };
        }

        public static string GetContentType(this string fileName)
        {
            if (!Provider.TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}