namespace MiddlewareSample
{
    public class MyCustomMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Console.WriteLine("before 2nd request");
            await context.Response.WriteAsync("Hello world from explicite-class use extention method. ");

            await next(context);
            Console.WriteLine("after 2nd request");
        }
    }
}
