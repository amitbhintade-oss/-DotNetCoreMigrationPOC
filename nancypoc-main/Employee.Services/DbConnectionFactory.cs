using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Employee.Services
{
    public class DbConnectionFactory
    {
        public static SqlConnection CreateConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing from configuration.");
            }

            try
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                connection.Close();
                return new SqlConnection(connectionString);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Unable to connect to database. Please check your connection string and ensure the database is accessible. Error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while creating database connection: {ex.Message}", ex);
            }
        }

        public static string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing from configuration.");
            }

            return connectionString;
        }
    }
}