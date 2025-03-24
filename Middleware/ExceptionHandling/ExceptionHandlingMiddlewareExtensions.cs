using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterChangeApi.Middleware;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MeterChangeApi.Middleware.ExceptionHandling
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}