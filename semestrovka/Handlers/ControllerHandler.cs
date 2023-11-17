using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using semestrovka.Attributes;
using semestrovka.Mapper;
using semestrovka.Services;
using semestrovka.TemplateEngine;
using semestrovka.utils;

namespace semestrovka.Handlers;

public class ControllerHandler : Handler
{
    public override void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        
        // Log the request url
        Console.WriteLine($"Request url: {request.Url}");
        
        // Getting controller
        var strParams = ParseUrlPath(request);
        var controller = GetController(Assembly.GetExecutingAssembly(), strParams[0]);
        if (controller != null && strParams.Length > 1)
        {
            try
            {
                // Getting methods of controller and action from url
                var methods = controller.GetMethods();
                var actionName = strParams[1];
                var methodType = request.HttpMethod;
                
                // Getting methods with matching action name
                var methodsByAction = GetMethodByAction(methods, actionName);
                
                // Getting methods with matching http method
                var methodsByHttpMethod = GetMethodsByHttpMethod(methodsByAction, methodType);
                var method = methodsByHttpMethod.Any()
                    ? methodsByHttpMethod.First()
                    : null;
                
                // Passing action execution to different method
                HandleDifferentMethods(context, methodType, controller, method);
            }
            catch (Exception e)
            {
                // Logging error and redirecting to home page if catching exception
                Console.WriteLine(e);
                // context.Response.Redirect("http://localhost:2323/");
                RedirectToHomePage(context);
            }
        }
        else
        {
            // Redirecting to home page if url is empty or to controller index page if url doesnt contain action name
            if (controller == null)
            {
                if (request.Url.LocalPath is "/" or "")
                {
                    RedirectToHomePage(context);
                }
                else
                {
                    context.Response.Redirect("http://localhost:2323/");
                    RedirectToHomePage(context);
                }
                
            }
            else
            {
                RedirectToControllerIndexPage(context, controller);
            }
        }
    }

    private MethodInfo[] GetMethodsByHttpMethod(MethodInfo[] methods, string method)
    {
        switch (method)
        {
            case "post":
            case "Post":    
            case "POST":
                return methods.Where(m => Attribute.IsDefined(m, typeof(PostAttribute))).ToArray();
            case "get":
            case "Get":    
            case "GET":
                return methods.Where(m => Attribute.IsDefined(m, typeof(GetAttribute))).ToArray();
        }

        return methods;
    }

    private Type? GetController(Assembly assembly, string controllerName)
    {
        return assembly.GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(ControllerAttribute)))
            .Select(i => new
            {
                AttributeName = i.GetCustomAttribute<ControllerAttribute>()?.ControllerName,
                Controller = i
            })
            .FirstOrDefault(c =>
                string.Equals(c.AttributeName, controllerName, StringComparison.CurrentCultureIgnoreCase))
            ?.Controller;
    }
    
    private MethodInfo[] GetMethodByAction(MethodInfo[] methods, string actionName)
    {
        return methods.Where(c => string.Equals(c.Name, actionName, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();
    }

    private string[] FetchFromFormData(HttpListenerRequest request)
    {
        string[] formData = null;
        using (var sr = new StreamReader(request.InputStream))
        {
            var tempData = sr.ReadToEnd();
            var decoded = WebUtility
                .UrlDecode(tempData);
            
            if (decoded.Contains('&'))
                return decoded.Split('&')
                .Select(param => param.Split('=')[1])
                .ToArray();

            return new [] { decoded.Split('=')[1] };

        }
    }

    private int? FetchParameter(string requestPath)
    {
        Console.WriteLine(requestPath);
        var pathSplitted = requestPath.Split('/').Skip(1).ToArray();
        if (pathSplitted.Length > 2)
        {
            var parameterToParse = pathSplitted[2];
            if (int.TryParse(parameterToParse, out var parameter))
                return parameter;
        }
        
        return null;
    }

    private object[]? ParseToMethodParams(MethodInfo method, string[] formData)
    {
        var methodParams = method.GetParameters();
        if (methodParams.Any())
            return methodParams.Select((p, i) => Convert.ChangeType(formData[i], p.ParameterType))
                .ToArray();
        else
            return null;
    }

    private string[] ParseUrlPath(HttpListenerRequest request)
    {
        return request.Url.LocalPath
            .Split('/')
            .Skip(1)
            .ToArray();
    }

    private async void RedirectToControllerIndexPage(HttpListenerContext context, Type controller)
    {
        var methods = controller.GetMethods();
        ResponseMessage? output = null;
        if (methods.Any())
            if (methods.Select(i => i.Name).Contains("Index"))
            {
                var method = methods.First(i => i.Name == "Index");
                if (Authorize(context, method))
                    output = (ResponseMessage) methods.First(i => i.Name == "Index")
                        .Invoke(null, new object[] { context })!;
            }

        if (output == null)
        {
            context.Response.Redirect("http://localhost:2323/");
            Console.WriteLine("Redirection to controller index page");
            RedirectToHomePage(context);
        }
        else
            await WriteOutputHtml(context, output.Message);
    }
    
    private async void RedirectToHomePage(HttpListenerContext context)
    {
        var content = PageMapper.MapHomePage();
        var response = context.Response;
        var buffer = Encoding.UTF8.GetBytes(content);
        response.ContentType = "text/html; charset=uts-8";
        response.ContentLength64 = buffer.Length;
        await using Stream output = response.OutputStream;

        await output.WriteAsync(buffer);
        await output.FlushAsync();
    }

    private async void WriteOutputSwitchMethods(HttpListenerContext context, string methodType,
        ResponseMessage responseMessage, string redirectionUrl = "")
    {
        if (responseMessage.Message.StartsWith("setcookie"))
        {
            var messageParsed = responseMessage.Message.Split(" ");
            if (messageParsed.Length == 3)
            {
                var key = messageParsed[1];
                var value = messageParsed[2];
                SetCookie(context, key, value);
            }
        }
        if (redirectionUrl != "")
        {
            Console.WriteLine($"Redirection to \'{redirectionUrl}\'");
            context.Response.Redirect(redirectionUrl.ToLower());
        }
        
        switch (methodType)
        {
            case "POST":
            case "Post":
            case "post":
            {
                await WriteOutput(context, responseMessage);
                break;
            }
            case "GET":
            case "Get":    
            case "get":
                await WriteOutputHtml(context, responseMessage.Message);
                break;
        }
    }
    
    private async Task WriteOutput(HttpListenerContext context, ResponseMessage responseMessage)
    {
        var response = context.Response;
        var content = $"<h1>Status Code: {responseMessage.StatusCode}</h1>" +
                      $"<h2>Message: {responseMessage.Message}</h2>" +
                      $"<a href=\"/\" style=\"font-size: 24px;\">Go To Home Page</a>";
        
        var buffer = Encoding.ASCII.GetBytes(content);
        response.ContentType = "text/html; charset=utf-8";
        response.ContentLength64 = buffer.Length;
        await using Stream output = response.OutputStream;
            
        await output.WriteAsync(buffer);
        await output.FlushAsync();
    }

    private async Task WriteOutputHtml(HttpListenerContext context, string content)
    {
        var response = context.Response;
        var buffer = Encoding.UTF8.GetBytes(content);
        response.ContentType = "text/html; charset=uts-8";
        response.ContentLength64 = buffer.Length;
        await using Stream output = response.OutputStream;

        await output.WriteAsync(buffer);
        await output.FlushAsync();
    }

    private void HandleDifferentMethods(HttpListenerContext context,
        string methodType,
        Type? controller,
        MethodInfo? method)
    {
        var request = context.Request;
        var isGetMethod = true;
        var controllerAttributeName = controller
            .GetCustomAttribute<ControllerAttribute>()
            .ControllerName;
        
        if (method == null || !Authorize(context, method))
        {
            context.Response.Redirect($"http://localhost:2323/{controllerAttributeName.ToLower()}");
            Console.WriteLine("Redirection to controller index page");
            RedirectToControllerIndexPage(context, controller);
        }
        else
        {
            object? methodResponse = null;
            switch (methodType)
            {
                case "post":
                case "Post":
                case "POST":
                {
                    var formData = FetchFromFormData(request);
                    methodResponse = HandlePostMethod(context, formData, method);
                    isGetMethod = false;
                    break;
                }
                case "get":
                case "Get":
                case "GET":
                {
                    var parameter = FetchParameter(context.Request.Url.LocalPath);
                    methodResponse = HandleGetMethod(context, parameter, method);
                    isGetMethod = true;
                    break;
                }
                     
            }

            if (methodResponse != null)
            {
                WriteOutputSwitchMethods(context,
                    methodType, 
                    (ResponseMessage) methodResponse, 
                    isGetMethod 
                        ? ""
                        : $"http://localhost:2323/{controllerAttributeName.ToLower()}");
            }
        }
    }

    private ResponseMessage HandlePostMethod(HttpListenerContext context, string[] formData, MethodInfo method)
    {
        object? methodResponse = null;
        if (formData.Any())
        {
            var queryParams = ParseToMethodParams(method, formData);
            methodResponse = method.Invoke(null, queryParams ?? Array.Empty<object>());
        }
        else
        {
            methodResponse = method.Invoke(null, Array.Empty<object>());
        }

        return methodResponse != null
            ? (ResponseMessage) methodResponse
            : new ResponseMessage(500, "Internal error");
    }

    private ResponseMessage HandleGetMethod(HttpListenerContext context, int? parameter, MethodInfo method)
    {
        object? methodResponse = null;
        Console.WriteLine($"Parameter - {parameter}");
        methodResponse = method.Invoke(null, parameter != null 
            ? new object[] { parameter } 
            : Array.Empty<object>());

        return methodResponse != null
            ? (ResponseMessage) methodResponse
            : new ResponseMessage(500, "Internal error");
    }

    private bool SetCookie(HttpListenerContext context, string key, string value)
    {
        
        var cookie = new Cookie
        {
            Expires = DateTime.Now.AddDays(1),
            Name = key,
            Value = value
        };
        context.Response.AppendCookie(cookie);
        return true;
    }

    private bool Authorize(HttpListenerContext context, MethodInfo method)
    {
        if (!Attribute.IsDefined(method, typeof(AuthorizeAttribute)))
            return true;

        var authService = new AuthService();
        var tokenTuple = GetAuthorizationCookieValue(context);
        return tokenTuple.Item1 && authService.ValidateToken(tokenTuple.Item2);
    }
    
    private (bool, string) GetAuthorizationCookieValue(HttpListenerContext context)
    {
        return GetCookieValue(context, "Authorization");
    }
    
    private (bool, string) GetCookieValue(HttpListenerContext context, string key)
    {
        var cookies = context.Request.Cookies
            .Where(i => i.Name == key)
            .ToArray();

        var cookie = cookies.Any()
            ? cookies.First()
            : null;
        
        return cookie != null
            ? (true, cookie.Value) 
            : (false, "No cookie found");
    }
}