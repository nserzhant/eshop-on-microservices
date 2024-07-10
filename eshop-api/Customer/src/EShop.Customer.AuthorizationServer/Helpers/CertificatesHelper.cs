using Microsoft.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace EShop.Customer.AuthorizationServer.Helpers;

public static class CertificatesHelper
{
    /// <summary>
    /// Find certificate by name (for Linux) or by thumbprint (for Windows)
    /// </summary>
    /// <param name="findCertificateValue">Certificate file name (Linux) or thumbprint (Windows)</param>
    /// <returns>Certificate</returns>
    /// <exception cref="Exception">Certificate Not Found Exception</exception>
    public static X509Certificate2 FindCertificate(string findCertificateValue)
    {
        if (OperatingSystem.IsWindows())
        {
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            return store.Certificates.Find(X509FindType.FindByThumbprint, findCertificateValue, validOnly: false)
              .OfType<X509Certificate2>()
              .SingleOrDefault() ?? throw new Exception("Certificate not found");
        }

        if (OperatingSystem.IsLinux())
        {
            var path = $"/var/ssl/private/{findCertificateValue}";

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
