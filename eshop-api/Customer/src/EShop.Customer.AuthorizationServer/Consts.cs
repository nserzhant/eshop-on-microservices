namespace EShop.Customer.AuthorizationServer;

public static class Consts
{
    public const string CUSTOMER_DB_SCHEMA_NAME = "customer";
    public const string CONNECTION_STRING_NAME = "customerDbConnectionString";
    public const string OpenIddict_DB_CONNECTION_STRING_NAME = "openIddictDbConnectionString";
    public const string ACCESS_TOKEN_LIFETIME_CONFIG_NAME = "tokenLifeTime";
    public const string CLIENT_CONFIGURATION_CONFIG_NAME = "clients";
    public const string INIT_DB_ON_STARTUP_CONFIG_NAME = "initDbOnStartup";
    public const string APP_BASE_PATH_ENVIRONMENT_NAME = "ASPNETCORE_APPL_PATH";
    public const string SIGNING_CERTIFICATE_CONFIG_NAME = "signingCertificate";
    public const string ENCRYPTION_CERTIFICATE_CONFIG_NAME = "encryptionCertificate";
    public const string USE_EPHEMERAL_KEYS = "useEphemeralKeys";
}
