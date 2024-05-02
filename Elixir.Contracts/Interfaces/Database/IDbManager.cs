using System;
using System.Data.OleDb;

namespace Elixir.Contracts.Interfaces.Database
{
    public interface IDbManager : IDisposable
    {
        OleDbCommand CreateCommand(String sql);
        OleDbConnection Connection { get; }
        OleDbCommand Command { get; }
    }
}
