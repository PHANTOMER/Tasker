using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using Tasker.DataContext.Extensions;

namespace Tasker.DataContext.DependencyListener
{
    public class SqlNotificationListener<TEntity> : IDisposable
        where TEntity : class
    {
        private SqlConnection _connection = null;
        private SqlCommand _command = null;
        private IQueryable _iquery = null;
        private readonly ObjectQuery _oquery = null;
        private DbContext _context;

        // Summary:
        //     Occurs when a notification is received for any of the commands associated
        //     with this ImmediateNotificationRegister object.
        public event EventHandler OnChanged;

        private SqlDependency _dependency = null;

        /// <summary>
        /// Initializes a new instance of ImmediateNotificationRegister class.
        /// </summary>
        /// <param name="query">an instance of ObjectQuery is used to get _connection string and 
        /// _command string to register SqlDependency nitification. </param>
        public SqlNotificationListener(ObjectQuery query)
        {
            try
            {
                this._oquery = query;
                
                _connection = QueryExtension.GetSqlConnection(_oquery);
                _command = QueryExtension.GetSqlCommand(query, _connection);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Paramter cannot be null", nameof(query), ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Fails to initialize a new instance of SqlNotificationListener class.", ex);
            }
        }

        /// <summary>
        /// Initializes a new instance of ImmediateNotificationRegister class.
        /// </summary>
        /// <param name="context">an instance of DbContext is used to get an ObjectQuery object</param>
        /// <param name="query">an instance of IQueryable is used to get ObjectQuery object, and then get  
        /// _connection string and _command string to register SqlDependency nitification. </param>
        public SqlNotificationListener(DbContext context, IQueryable query)
        {
            try
            {
                this._iquery = query;
                _context = context;

                // Get the ObjectQuery directly or convert the DbQuery to ObjectQuery.
                _oquery = QueryExtension.GetObjectQuery<TEntity>(context, _iquery);

                _connection = QueryExtension.GetSqlConnection(_oquery);
                _command = QueryExtension.GetSqlCommand(_oquery, _connection);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "context")
                {
                    throw new ArgumentException("Paramter cannot be null", nameof(context), ex);
                }

                throw new ArgumentException("Paramter cannot be null", nameof(query), ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Fails to initialize a new instance of SqlNotificationListener class.", ex);
            }
        }

        /// <summary>
        /// Starts the notification of SqlDependency 
        /// </summary>
        public void StartMonitor()
        {
            try
            {
                SqlDependency.Start(_context.Database.Connection.ConnectionString);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Fails to Start the SqlDependency in the SqlNotificationListener class", ex);
            }
        }

        /// <summary>
        /// Stops the notification of SqlDependency 
        /// </summary>
        public void StopMonitor()
        {
            try
            {
                SqlDependency.Stop(_context.Database.Connection.ConnectionString);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Fails to Stop the SqlDependency in the SqlNotificationListener class", ex);
            }
        }

        public void RegisterSqlDependency()
        {
            if (_command == null || _connection == null)
            {
                throw new ArgumentException("_command and _connection cannot be null");
            }

            // Make sure the _command object does not already have
            // a notification object associated with it.
            _command.Notification = null;

            // Create and bind the SqlDependency object to the _command object.
            _dependency = new SqlDependency(_command);
            _dependency.OnChange += new OnChangeEventHandler(DependencyOnChange);

            // After register SqlDependency, the SqlCommand must be executed, or we can't 
            // get the notification.
            RegisterSqlCommand();
        }

        private void DependencyOnChange(object sender, SqlNotificationEventArgs e)
        {
            // Move the original SqlDependency event handler.
            SqlDependency dependency = (SqlDependency)sender;
            dependency.OnChange -= DependencyOnChange;

            var handler = OnChanged;

            if (handler != null)
            {
                handler(this, null);
            }

            // We re-register the SqlDependency.
            RegisterSqlDependency();
        }

        private void RegisterSqlCommand()
        {
            if (_connection != null && _command != null)
            {
                _connection.Open();
                _command.ExecuteNonQuery();
                _connection.Close();
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposed)
        {
            if (disposed)
            {
                if (_command != null)
                {
                    _command.Dispose();
                    _command = null;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }

                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }

                OnChanged = null;
                _iquery = null;
                _dependency.OnChange -= DependencyOnChange;
                _dependency = null;
            }
        }

        /// <summary>
        /// The SqlConnection is got from the Query.
        /// </summary>
        public SqlConnection Connection => _connection;

        /// <summary>
        /// The SqlCommand is got from the Query.
        /// </summary>
        public SqlCommand Command => _command;

        /// <summary>
        /// The ObjectQuery is got from the Query.
        /// </summary>
        public ObjectQuery Oquery => _oquery;
    }
}
