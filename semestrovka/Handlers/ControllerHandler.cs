using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using semestrovka.Attributes;
using semestrovka.utils;

namespace semestrovka.Handlers;

public class ControllerHandler : Handler
{
    public override void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var strParams = ParseUrlPath(request);
        if (strParams.Length == 0)
            RedirectToHomePage(context);
        var controller = GetController(Assembly.GetExecutingAssembly(), strParams[0]);
        if (controller != null && strParams.Length > 1)
        {
            try
            {
                var methods = controller.GetMethods();
                var actionName = strParams[1];
                var methodType = request.HttpMethod;
                var methodsByAction = GetMethodByAction(methods, actionName);
                var method = GetMethodsByHttpMethod(methodsByAction, methodType).First();
                HandleDifferentMethods(context, methodType, method);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToHomePage(context);
            }
        }
        else
        {
            RedirectToHomePage(context);
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
            return WebUtility
                .UrlDecode(tempData)
                .Split('&')
                .Select(param => param.Split('=')[1])
                .ToArray();
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

    private void RedirectToControllerIndexPage(HttpListenerContext context)
    {
        
    }
    
    private void RedirectToHomePage(HttpListenerContext context)
    {
        
    }

    private async void WriteOutputSwitchMethods(HttpListenerContext context, string methodType,
        ResponseMessage responseMessage)
    {
        switch (methodType)
        {
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
        MethodInfo method)
    {
        var request = context.Request;
        
        if (method == null)
            RedirectToControllerIndexPage(context);
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
                    break;
                }
                case "get":
                case "Get":
                case "GET":
                {
                    var parameter = FetchParameter(context.Request.Url.LocalPath);
                    methodResponse = HandleGetMethod(context, parameter, method);
                    break;
                }
                     
            }

            if (methodResponse != null)
            {
                WriteOutputSwitchMethods(context, methodType, (ResponseMessage) methodResponse);
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
            ? (ResponseMessage)methodResponse
            : new ResponseMessage(500, "Internal error");
    }

    private ResponseMessage HandleGetMethod(HttpListenerContext context, int? parameter, MethodInfo method)
    {
        object? methodResponse = null;
        Console.WriteLine($"parameter - {parameter}");
        methodResponse = method.Invoke(null, parameter != null 
            ? new object[] { parameter } 
            : Array.Empty<object>());

        return methodResponse != null
            ? (ResponseMessage)methodResponse
            : new ResponseMessage(500, "Internal error");
    }
}