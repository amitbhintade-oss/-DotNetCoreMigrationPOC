using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Employee.Services.DapperBase
{
    public class DapperBase
    {
        private readonly string _connectionString;

        public DapperBase(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(sql, parameters);
            }
        }

        protected async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
            }
        }

        protected async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(sql, parameters);
            }
        }

        protected async Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<T>(sql, parameters);
            }
        }

        public async Task<T> QuerySingleOrDefaultPublicAsync<T>(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
            }
        }
    }
}