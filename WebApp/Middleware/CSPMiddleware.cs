namespace WebApp.Middleware
{
    public class CSPMiddleware
    {
        private readonly RequestDelegate _next;
        public CSPMiddleware(RequestDelegate next)
        {
            _next = next;  
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            context.Items["CSP-Nonce"] = nonce;
            context.Response.Headers["Content-Security-Policy"] =
     "default-src 'self'; " +
     "script-src 'self' https://trusted.cdn.com 'nonce-" + nonce + "'; " +
     "style-src 'self' https://trusted.cdn.com 'nonce-" + nonce + "'; " +
     "img-src 'self' data:; " +
     "font-src 'self' https://trusted.cdn.com; " +
     "connect-src 'self'; " +
     "frame-src 'self' 'nonce-" +nonce + "' ; " +
     "frame-ancestors 'self'  'nonce-" + nonce + "' ; " +
     "object-src 'none'; " +
     "base-uri 'self'; " +
     "form-action 'self';";
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
            await _next(context);
        }
    }
}
