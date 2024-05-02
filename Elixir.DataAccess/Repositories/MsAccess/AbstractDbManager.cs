using System;
using System.Data.OleDb;
using Elixir.Contracts.Interfaces.Database;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public abstract class AbstractDbManager : IDbManager
    {
        protected string DatabaseFilePath = @"";

        protected string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=G:\\elixir.accdb";

        protected AbstractDbManager()
        {
            InitializeConnection();

            Connection = new OleDbConnection(ConnectionString);
            Connection.Open();
        }

        protected abstract void InitializeConnection();

        public OleDbCommand CreateCommand(String sql)
        {
            return Command ?? (Command = new OleDbCommand(sql) { Connection = Connection });
        }

        public void Dispose()
        {
            Connection.Close();
        }

        public OleDbConnection Connection { get; private set; }
        public OleDbCommand Command { get; private set; }
    }
}
