namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class MsAccessDbManager : AbstractDbManager
    {
        protected override void InitializeConnection()
        {
            DatabaseFilePath = @"|DataDirectory|/elixir.accdb";

            ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=G:\\elixir.accdb";
        }
    }
}
