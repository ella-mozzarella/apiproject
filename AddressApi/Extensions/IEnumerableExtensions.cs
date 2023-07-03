using System.Linq.Expressions;
using System.Reflection;

namespace AddressApi.Extensions
{    
  /// <summary>
  /// Class <c>IEnumerableExtensions</c> is used for extending IEnumerable
  /// </summary>
  public static class IEnumerableExtensions
  {
    /// <summary>
    /// Method <c>OrderBy</c> extends OrderBy to sort given enumerable by a given property and order by asc/desc
    /// </summary>
    public static IEnumerable<TEntity> OrderBy<TEntity>(this IEnumerable<TEntity> source, string sortBy, bool isAscending)
    {
      string methodName = isAscending ? "OrderBy" : "OrderByDescending"; //determine which method to use (asc/desc)

      PropertyInfo? property = typeof(TEntity).GetProperty(sortBy); //property to sort by

      //if property isn't found (case sensitive!) return unsorted
      if (property == null)
        return source;

      ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "p"); //parameterize expression
      MemberExpression property_access = Expression.MakeMemberAccess(parameter, property); //access the property in the parameterized expression
      LambdaExpression orderby_expression = Expression.Lambda(property_access, parameter); //lambda expression 
      MethodCallExpression result_expression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(TEntity), property.PropertyType }, source.AsQueryable().Expression, Expression.Quote(orderby_expression)); //method call, orderBy(property), orderByDescending(property)
      
      return source.AsQueryable().Provider.CreateQuery<TEntity>(result_expression); //return enumerable sorted according to expression
    }
  }
}
