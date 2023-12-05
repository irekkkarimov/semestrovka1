using System.Net;
using semestrovka.DAOs;

namespace semestrovka.Services;

public class CookiesHandlerService : ICookiesHandlerService
{
    
    /// <summary>
    /// Fetches authorization token from the cookies for current url path
    /// </summary>
    /// <returns>(bool, string) tuple, where: bool - if fetching succeeded,
    /// string - token if succeeded</returns>
    public (bool, string) FetchTokenFromCookies(HttpListenerContext context)
    {
        var cookies = context.Request.Cookies;

        var authorizationCookie = cookies.FirstOrDefault(i => i.Name == "Authorization");

        return authorizationCookie != null
            ? (true, authorizationCookie.Value)
            : (false, "");
    }

    public (bool, int) FetchTokenFromCookiesAndGetUserId(HttpListenerContext context, IAuthService authService, IUserDao userDao)
    {
        var tokenTuple = FetchTokenFromCookies(context);
        if (!tokenTuple.Item1)
            return (false, 0);

        var userIdTuple = authService.ValidateTokenAndGetUserId(userDao, tokenTuple.Item2);
        
        return !userIdTuple.Item1 
            ? (false, 0) 
            : (true, userIdTuple.Item2);
    }
}