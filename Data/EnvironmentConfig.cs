namespace Magazynek.Data
{
    public static class EnvironmentConfig
    {
        public static string GetPostgresConnectionString()
        {
            DotNetEnv.Env.Load();
            return $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                   $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                   $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
        }
    }
}