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
            context.Response.Headers["Content-Security-Policy"] =
    "default-src 'self'; " +
    "script-src 'self' https://trusted.cdn.com; " +
    "style-src 'self' https://trusted.cdn.com; " +
    "img-src 'self' data:; " +
    "font-src 'self' https://trusted.cdn.com; " +
    "connect-src 'self'; " +
    "frame-src 'self'; " +
    "object-src 'none'; " +
    "base-uri 'self'; " +
    "form-action 'self';";
            await _next(context);
        }
    }
}
