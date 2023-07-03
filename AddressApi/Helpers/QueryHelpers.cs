using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AddressApi.Helpers
{    
    /// <summary>
    /// Class <c>QueryHelpers</c> is used for helping with DbSet queries
    /// </summary>
    public static class QueryHelpers
    {
        /// <summary>
        /// Method <c>CreateSearchQuery</c> dynamically builds an expression to check whether any string property of the given DbSet contains the given value.
        /// </summary>
        public static IQueryable<T> CreateSearchQuery<T>(DbSet<T> db_set , string value) where T:class
        {
            IQueryable<T> query = db_set; //query to search
            List<Expression> expressions_list = new List<Expression>(); //list to hold expressions
            ParameterExpression parameter = Expression.Parameter(typeof (T), "p"); //parameterize expression
            MethodInfo contains_method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!; //method to call is contains

            //for each property of type string in the DbSet
            foreach (PropertyInfo prop in typeof(T).GetProperties().Where(x => x.PropertyType == typeof (string)))
            {
                MemberExpression member_expression = Expression.PropertyOrField(parameter, prop.Name); //the parameterized expression and name of property to search
                ConstantExpression value_expression = Expression.Constant(value, typeof(string)); //the value to search for
                MethodCallExpression contains_expression = Expression.Call(member_expression, contains_method, value_expression); //method call, property.contains(value)
                expressions_list.Add(contains_expression); //add the expression to the list
            }

            //if expressions list empty then nothing to search through so just return query
            if (expressions_list.Count == 0)
                return query;

            //add first expression as primary condition
            Expression or_expression = expressions_list[0];

            //for each expression after the first
            for (int i = 1; i < expressions_list.Count; i++)
            {
                or_expression = Expression.OrElse(or_expression, expressions_list[i]); //add expression as (OR) secondary condition
            }

            Expression<Func<T, bool>> result_expression = Expression.Lambda<Func<T, bool>>(or_expression, parameter); //define final expression

            return query.Where(result_expression); //return query where expression = true
        }
    }
}