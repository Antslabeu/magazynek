using Magazynek.Data.Mailer;

namespace Magazynek.Data
{
    public static class EnvironmentConfig
    {
        public static string GetPostgresConnectionString(string envFileName = "main.env")
        {
            DotNetEnv.Env.Load(envFileName);
            return $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                   $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                   $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                   $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                   $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
        }
        public static SmtpOptions GetSmtpOptions(string envFileName = "main.env")
        {
            DotNetEnv.Env.Load(envFileName);
            return new SmtpOptions()
            {
                Host = Environment.GetEnvironmentVariable("MAIL_HOST")!,
                Port = int.Parse(Environment.GetEnvironmentVariable("MAIL_PORT")!),
                User = Environment.GetEnvironmentVariable("MAIL_USER")!,
                Pass = Environment.GetEnvironmentVariable("MAIL_PASS")!,
                FromAddress = Environment.GetEnvironmentVariable("MAIL_FROM_ADDRESS")!,
                FromName = Environment.GetEnvironmentVariable("MAIL_FROM_NAME")!,
                UseImplicitTls = Environment.GetEnvironmentVariable("MAIL_USE_TLS")! == "true"
            };
        } 
    }
}