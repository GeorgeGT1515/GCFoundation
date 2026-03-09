using GCFoundation.Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace GCFoundation.Security.Middlewares
{
    public class GCFoundationContentPoliciesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GCFoundationContentPolicySettings _settings;

        public GCFoundationContentPoliciesMiddleware(RequestDelegate next, IOptions<GCFoundationContentPolicySettings> settings)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));

            _next = next;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            // Generate a nonce for inline styles/scripts (if needed)
            string nonce = GenerateNonce();
            context.Items["CspNonce"] = nonce; // Store for use in views (if required)

            // Convert lists to space-separated strings
            string connectCDN = string.Join(" ", _settings.ConnectCDN ?? Enumerable.Empty<string>());
            string cssCDN = string.Join(" ", _settings.CssCDN ?? Enumerable.Empty<string>());
            string cssCDNHash = string.Join(" ", _settings.CssCDNHash ?? Enumerable.Empty<string>());
            string fontCDN = string.Join(" ", _settings.FontCDN ?? Enumerable.Empty<string>());
            string jsCDN = string.Join(" ", _settings.JavascriptCDN ?? Enumerable.Empty<string>());

            // Build Content Security Policy (CSP)
            string contentSecurityPolicy = $"default-src 'none'; " +
                               $"script-src 'self' {jsCDN} 'nonce-{nonce}'; " +
                               $"object-src 'none'; " +
                               $"style-src 'self' 'unsafe-hashes' {cssCDN} {cssCDNHash} 'nonce-{nonce}'; " +
                               $"font-src 'self' {fontCDN}; " +
                               $"connect-src 'self' {connectCDN} http://localhost:* https://localhost:* ws://localhost:* wss://localhost:* https://cdn.design-system.alpha.canada.ca; " +
                               $"img-src 'self' data:; " +
                               $"frame-ancestors 'none'; " +
                               $"upgrade-insecure-requests;";

            // Set security headers
            context.Response.Headers.Append("Content-Security-Policy", contentSecurityPolicy);
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains"); // 1 year HSTS
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            context.Response.Headers.Append("Expect-CT", "max-age=86400, enforce");
            context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");

            // Set Cross-Origin Policies.
            // COEP "credentialless" allows cross-origin resources (e.g. GCDS CSS from cdn.design-system.alpha.canada.ca)
            // to load without requiring CORP/CORS from the CDN, while still isolating them (no credentials sent).
            // "require-corp" would block GCDS/CDN styles unless the CDN sends Cross-Origin-Resource-Policy.
            context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
            context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
            context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "credentialless");

            await _next(context).ConfigureAwait(false);
        }

        private static string GenerateNonce()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] nonceBytes = new byte[16];
                rng.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes);
            }
        }
    }
}
