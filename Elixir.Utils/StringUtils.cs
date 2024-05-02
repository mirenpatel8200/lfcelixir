namespace Elixir.Utils
{
    public static class StringUtils
    {
        public static string FixSqlLikeClause(string sqlClause)
        {
            return sqlClause.Replace("[", "[[]");
        }
    }
}
