using Microsoft.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace EShop.Client.AuthorizationServer.Helpers;

public static class CertificatesHelper
{
    public static X509Certificate2 FindCertificate(string thumbprint)
    {
        if (OperatingSystem.IsWindows())
        {
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            return store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false)
              .OfType<X509Certificate2>()
              .SingleOrDefault() ?? throw new Exception("Certificate not found");
        }

        if (OperatingSystem.IsLinux())
        {
            var path = $"/var/ssl/private/{thumbprint}.p12";

            if (File.Exists(path))
            {
                var bytes = File.ReadAllBytes(path);
                var cert = new X509Certificate2(bytes);
                return cert;
            }
        }

        throw new Exception("Certificate not found");
    }
}
