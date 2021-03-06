﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Sharporum.Core.Responses;
using Sharporum.Domain.Models;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Helpers
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

        public static bool IsPaginatonSearchParamsValid(BaseSearchParams searchParams,
            out ErrorResponse errorResponse)
        {
            errorResponse = new ErrorResponse();
            var errors = new List<ErrorModel>();
            if ((searchParams.Limit > 50) || (searchParams.Limit <= 0))
            {
                errors.Add(new ErrorModel
                {
                    Message = "Limit must be between 0 and 50",
                    Name = nameof(searchParams.Limit),
                });
            }

            if (searchParams.CurrentPage <= 0)
            {
                errors.Add(new ErrorModel
                {
                    Message = "Current Page must not be negative",
                    Name = nameof(searchParams.CurrentPage),
                });
            }

            if (!string.IsNullOrEmpty(searchParams.SortBy) && !searchParams.SortBy.Equals("CreatedAt") &&
                !searchParams.SortBy.Equals("VoteCount"))
            {
                errors.Add(new ErrorModel
                {
                    Message = "Can not sort by other values than \"CreatedAt\" or \"VoteCount\"",
                    Name = nameof(searchParams.SortBy),
                });
            }

            foreach (ErrorModel errorModel in errors.Select(error => new ErrorModel
            {
                Name = error.Name,
                Message = error.Message,
            }))
            {
                errorResponse.Errors.Add(errorModel);
            }

            return !errors.Any();
        }

        public static FileData GetContentFileData<TEntity>(string content, string fileName)
        {
            // TODO: add validation
            string[] contentParts = content.Split(",");
            string contentType = contentParts[0].Split("/")[1].Split(";")[0];
            string blobName = $"{typeof(TEntity).Name}/{fileName}.{contentType}";
            return new FileData
            {
                Content = contentParts[1],
                ContentType = blobName.GetContentType(),
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