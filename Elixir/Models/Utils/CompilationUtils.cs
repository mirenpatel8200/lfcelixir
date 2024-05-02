namespace Elixir.Models.Utils
{
    public static class CompilationUtils
    {
        public static bool IsProfilingMode
        {
            get
            {
#if PROFILING_MODE
                return true;
#else
                return false;
#endif
            }
        }
    }
}