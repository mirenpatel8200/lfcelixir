using System.Reflection;
using Elixir.DataAccess.Repositories.MsAccess;
using Elixir.Utils.Reflection;

namespace Elixir.xUnit
{
    public class UnitTestDbManager : AbstractDbManager
    {
        protected override void InitializeConnection()
        {
            var path = Assembly.GetExecutingAssembly().GetAssemblyDirectory();

            DatabaseFilePath = $@"{path}/../../../Elixir/App_Data/elixir.accdb";

            ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DatabaseFilePath;
        }
    }
}
