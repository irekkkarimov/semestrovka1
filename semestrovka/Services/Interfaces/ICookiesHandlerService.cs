using System.Net;
using semestrovka.DAOs;

namespace semestrovka.Services;

public interface ICookiesHandlerService
{
    (bool, string) FetchTokenFromCookies(HttpListenerContext context);
    (bool, int) FetchTokenFromCookiesAndGetUserId(HttpListenerContext context, IAuthService authService, IUserDao userDao);
}