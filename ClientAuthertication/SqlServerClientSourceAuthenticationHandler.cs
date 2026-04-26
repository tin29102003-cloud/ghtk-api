using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAuthertication
{
    public class SqlServerClientSourceAuthenticationHandler : IClientSourceAuthenticationHandler,IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection connection;
        private bool disposedValue;

        public SqlServerClientSourceAuthenticationHandler(string connectionString)
        {
            _connectionString = connectionString;
            connection = new SqlConnection(_connectionString);
        }
        public bool Validate(string clientSource)
        {
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
          

            using var command = connection.CreateCommand();
            command.CommandText = "  SELECT TOP 1 1 FROM ClientSources WHERE ClientId = @clientID AND GETDATE() >= ValidFrom AND GETDATE() <= ValidTo AND isEnable = 1 ";
            command.Parameters.AddWithValue("@clientID", clientSource);
            using var reader = command.ExecuteReader();
            if(reader.Read())
            {
                
                return true;
            }
         
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();

                    }
                    connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
