using System;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Tasker.DataContext.Extensions
{
    public static class QueryExtension
    {
        /// <summary>
        /// Return the ObjectQuery directly or convert the DbQuery to ObjectQuery.
        /// </summary>
        public static ObjectQuery GetObjectQuery<TEntity>(DbContext context, IQueryable query)
            where TEntity : class
        {
            var objectQuery = query as ObjectQuery;
            if (objectQuery != null)
                return objectQuery;

            if (context == null)
                throw new ArgumentException("Paramter cannot be null", nameof(context));

            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;

            IQueryable iqueryable = objectContext.CreateObjectSet<TEntity>() as IQueryable;
            IQueryProvider provider = iqueryable.Provider;

            return provider.CreateQuery(query.Expression) as ObjectQuery;
        }

        /// <summary>
        /// Use ObjectQuery to get SqlConnection and SqlCommand.
        /// </summary>
        public static SqlCommand GetSqlCommand(ObjectQuery query, SqlConnection connection)
        {
            if (query == null)
                throw new System.ArgumentException("Parameter cannot be null", nameof(query));

            SqlCommand command = new SqlCommand(QueryExtension.GetSqlString(query), connection);

            // Add all the paramters used in query.
            foreach (ObjectParameter parameter in query.Parameters)
            {
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);
            }

            return command;
        }

        public static SqlConnection GetSqlConnection(ObjectQuery query)
        {
            if (query == null)
                throw new System.ArgumentException("Parameter cannot be null", nameof(query));

            return new SqlConnection(QueryExtension.GetConnectionString(query));
        }

        /// <summary>
        /// Use ObjectQuery to get the connection string.
        /// </summary>
        public static String GetConnectionString(ObjectQuery query)
        {
            if (query == null)
            {
                throw new ArgumentException("Paramter cannot be null", nameof(query));
            }

            EntityConnection connection = query.Context.Connection as EntityConnection;
            if (connection == null)
                throw new Exception("Connection object must derive from EntityConnection");

            return connection.StoreConnection.ConnectionString;
        }

        /// <summary>
        /// Use ObjectQuery to get the Sql string.
        /// </summary>
        public static String GetSqlString(ObjectQuery query)
        {
            if (query == null)
            {
                throw new ArgumentException("Paramter cannot be null", nameof(query));
            }

            string s = query.ToTraceString();

            return s;
        }
    }
}
