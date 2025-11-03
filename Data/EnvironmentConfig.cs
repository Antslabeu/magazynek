namespace Magazynek.Data
{
    public static class EnvironmentConfig
    {
        public static string GetPostgresConnectionString(string envFileName)
        {
            DotNetEnv.Env.Load(envFileName);
            return $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                   $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                   $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                   $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
        }
    }
}