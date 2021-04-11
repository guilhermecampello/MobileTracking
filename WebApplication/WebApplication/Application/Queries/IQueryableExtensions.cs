using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Application
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Where<T, T2>(
            this IQueryable<T> queryable,
            T2? optionalFilter,
            Func<T2, Expression<Func<T, bool>>> predicateBuilder)
            where T2 : class
        {
            if (optionalFilter == null)
            {
                return queryable;
            }

            return queryable.Where(predicateBuilder.Invoke(optionalFilter));
        }

        public static IQueryable<T> Where<T, T2>(
            this IQueryable<T> queryable,
            T2? optionalFilter,
            Func<T2, Expression<Func<T, bool>>> predicateBuilder)
            where T2 : struct
        {
            if (!optionalFilter.HasValue)
            {
                return queryable;
            }

            return queryable.Where(predicateBuilder.Invoke(optionalFilter.Value));
        }

        public static IQueryable<T> Include<T, T2>(
            this IQueryable<T> query, bool include, Expression<Func<T, T2>> expression)
            where T : class
        {
            if (include)
            {
                query = query.Include(expression);
            }

            return query;
        }

        public static IQueryable<T1> Include<T1, T2, T3>(
            this IQueryable<T1> query,
            bool include,
            Expression<Func<T1, IEnumerable<T2>>> expression,
            Expression<Func<T2, IEnumerable<T3>>> secondExpression)
            where T1 : class
        {
            if (include)
            {
                query = query.Include(expression)
                    .ThenInclude(secondExpression);
            }

            return query;
        }

        public static IQueryable<T1> Include<T1, T2, T3>(
            this IQueryable<T1> query,
            bool include,
            Expression<Func<T1, T2>> expression,
            Expression<Func<T2, IEnumerable<T3>>> secondExpression)
            where T1 : class
        {
            if (include)
            {
                query = query.Include(expression)
                    .ThenInclude(secondExpression);
            }

            return query;
        }

        public static IQueryable<T1> Include<T1, T2, T3>(
            this IQueryable<T1> query,
            bool include,
            Expression<Func<T1, T2>> expression,
            Expression<Func<T2, T3>> secondExpression)
            where T1 : class
        {
            if (include)
            {
                query = query.Include(expression)
                    .ThenInclude(secondExpression);
            }

            return query;
        }
    }
}
