using MiddlewareSample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<MyCustomMiddleware>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Inline middlewares
// Below code show how can we write inline middleware
// Use extention method is used to add middleware to request pipeline. This extension method takes two parameters as below:
        // 1. context of type HttpContext to access HTTP request and response.
        // 2. Func type parameter i.e. delegate to handle the request or call next middlewaer in the pipeline
// while using Use extension method, you always need to call the next middleware component using next parameter
app.Use(async (context, next) =>
{
    //before logic
    Console.WriteLine("before 1st request");
    await context.Response.WriteAsync("Hello world from inline use extention method. ");

    await next();
    //after logic
    Console.WriteLine("after 1st request");
});

// Here it will print "before 1st request", and perform the next middleware in the pipeline and then it will return again to this middleware to perform after logic i.e. to print "after 1st request"

// Middleware using explicit class
// You can also implement custome middleware using separate class. For that, you can create class that implements IMiddleware interface with InvokeAsync method
// Refer MyCustomMiddleware class for the same.
// Once you provide midlleware logic inside a class, you need to register the class in the registry. Here it is registered at line 11.
// after registering the class, you need to provide that class to the UseMiddleware extention method as below
app.UseMiddleware<MyCustomMiddleware>();

// Here it will print "before 2nd request", and perform the next middleware in the pipeline and then it will return again to this middleware to perform after logic i.e. to print "after 2nd request" and
// eventually it will go to previous middleware to perform it's after logic (which is to print "after 1st request")

// Map extension method
// This is implemented as extension method of the IApplicationBuilder interface and that's why we are able to call Run() method using IApplicationBuilder instance i.e. app.
// Map method is used to perform some logic based on routes. The Map() method will branch out the request into two different pipelines.
// You can mention the path of a route as first parameter and function for the logic as second parameter.
app.Map("/map", MapHandler);
void MapHandler(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        Console.WriteLine("Hello from map method");
        await context.Response.WriteAsync("Hello from map method");
    });
}

// if you are getting output two times in console, remove extra call for favicon
app.Map("/favicon.ico", (app) => { });


// As we discussed above, you need to call the next middleware component using next parameter in case of Use extension method.
// But Run() extension method will terminate the request processing pipeline. The below code will not call the next middlewares, if any.
app.Run(async context => {
    Console.WriteLine("hello world");
    await context.Response.WriteAsync("Hello World from inline run extention method");
});


// If you run the application, by-default it will open swagger page. If you remove the swagger/index.html path from the url, you will be able to see the output as below:

// before 1st request  
// before 2nd request
// hello world
// after 2nd request
// after 1st request


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();


