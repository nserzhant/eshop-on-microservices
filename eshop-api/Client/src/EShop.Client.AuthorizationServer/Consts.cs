namespace EShop.Client.AuthorizationServer;

public static class Consts
{
    public const string CLIENT_DB_SCHEMA_NAME = "client";
    public const string CONNECTION_STRING_NAME = "clientDbConnectionString";
    public const string JWT_SECRET_KEY_CONFIG_NAME = "clientSTSJWTSecretKey";
    public const string ACCESS_TOKEN_LIFETIME_CONFIG_NAME = "tokenLifeTime";
    public const string CLIENT_CONFIGURATION_CONFIG_NAME = "clients";
    public const string INIT_DB_ON_STARTUP_CONFIG_NAME = "initDbOnStartup";
    public const string APP_BASE_PATH_ENVIRONMENT_NAME = "ASPNETCORE_APPL_PATH";
}
