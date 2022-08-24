using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;

namespace Quoridor.DataAccess.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string connectionString;

        protected BaseRepository(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected List<T> Query<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return new List<T>(connection.Query<T>(sql, parameters));
            }
        }

        protected async Task<List<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<T>(sql, parameters);
                return new List<T>(result);
            }
        }

        protected T QuerySingle<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.QuerySingle<T>(sql, parameters);
            }
        }

        protected async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QuerySingleAsync<T>(sql, parameters);
            }
        }

        protected T QueryFirstOrDefault<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<T>(sql, parameters);
            }
        }

        protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
        }

        protected List<T> QueryStoredProcedure<T>(string storedProcedureName, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return new List<T>(connection.Query<T>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure));
            }
        }

        protected async Task<List<T>> QueryStoredProcedureAsync<T>(string storedProcedureName, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<T>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                return new List<T>(result);
            }
        }

        protected int Execute(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.Execute(sql, parameters);
            }
        }

        protected async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.ExecuteAsync(sql, parameters);
            }
        }

        protected int ExecuteStoredProcedure(string storedProcedureName, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        protected async Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.ExecuteAsync(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        protected T ExecuteScalar<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.ExecuteScalar<T>(sql, parameters);
            }
        }

        protected async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.ExecuteScalarAsync<T>(sql, parameters);
            }
        }

        protected T ExecuteScalarStoredProcedure<T>(string storedProcedureName, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.ExecuteScalar<T>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        protected async Task<T> ExecuteScalarStoredProcedureAsync<T>(string storedProcedureName, object? parameters = null)
        {
            using (DbConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.ExecuteScalarAsync<T>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        /*
        protected T TraceAction<T>(System.Diagnostics.ActivitySource activitySource, string className, Func<T> method, [CallerMemberName] string? callerMemberName = null)
        {
            var activity = activitySource.StartActivity($"{className}.{callerMemberName}", ActivityKind.Internal);

            try
            {
                return method();
            }
            finally
            {
                activity?.Stop();
            }
        }

        protected void TraceAction(ActivitySource activitySource, string className, Action method, [CallerMemberName] string? callerMemberName = null)
        {
            var activity = activitySource.StartActivity($"{className}.{callerMemberName}", ActivityKind.Internal);

            try
            {
                method();
            }
            finally
            {
                activity?.Stop();
            }
        }
        */
    }
}
