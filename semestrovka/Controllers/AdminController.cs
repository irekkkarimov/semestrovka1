using System.Net;
using System.Text.RegularExpressions;
using semestrovka.Attributes;
using semestrovka.DAOs;
using semestrovka.DTOs;
using semestrovka.Handlers;
using semestrovka.Mapper;
using semestrovka.Services;
using semestrovka.TemplateEngine;
using semestrovka.utils;

namespace semestrovka.Controllers;

[Controller("Admin")]
public class AdminController
{
    private readonly HttpListenerContext _context;
    private readonly AuthService _authService = new();
    private readonly CookiesHandlerService _cookiesHandlerService = new();

    public AdminController(HttpListenerContext context)
    {
        _context = context;
    }

    [Authorize]
    [Get("Index")]
    public ResponseMessage Index(HttpListenerContext context)
    {
        var cookie = context.Request.Cookies.First(i => i.Name == "Authorization");
        var users = new UserDao().GetUsers().Select(i => new { i.Id, token = i.Email + i.Password });
        var userId = users.First(i => i.token == cookie.Value).Id;
        var mappedPage = PageMapper.MapAdminPage(userId);
        return mappedPage != ""
            ? new ResponseMessage(200, mappedPage)
            : new ResponseMessage(404, "Couldn't load admin page");
    }

    [Get("Login")]
    public ResponseMessage Login()
    {
        var page = StaticFilesHandler.FetchStaticFile("login.html");
        return new ResponseMessage(200, page);
    }

    [Post("Login")]
    public ResponseMessage Login(string email, string password)
    {
        var userToAuthorize = new UserDto
        {
            Email = email,
            Password = password
        };
        var user = UserMapper.MapUserDtoToUser(userToAuthorize);
        var userDao = new UserDao();
        var userFromDb = userDao.GetUserByEmail(user);
        if (user.Email == userFromDb.Email)
            if (user.Password == userFromDb.Password)
            {
                var authService = new AuthService();
                var token = authService.GenerateToken(user);
                return new ResponseMessage(200, $"setcookie Authorization {token}");
            }

        return new ResponseMessage(404, "Wrong email");
    }

    [Get("Logout")]
    public ResponseMessage Logout()
    {
        return new ResponseMessage(200, "setcookie Authorization _");
    }

    [Get("Registration")]
    public ResponseMessage Registration()
    {
        var page = StaticFilesHandler.FetchStaticFile("registration.html");
        return new ResponseMessage(200, page);
    }

    [Post("Registration")]
    public ResponseMessage Registration(string email, string password)
    {
        var regex = new Regex("([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\\.[a-zA-Z0-9_-]+)");

        if (!regex.IsMatch(email))
            return new ResponseMessage(401, "Wrong email");
        
        var userToRegister = new UserDto
        {
            Email = email,
            Password = password
        };
        var user = UserMapper.MapUserDtoToUser(userToRegister);
        var userDao = new UserDao();
        return userDao.CreateUser(user)
            ? new ResponseMessage(200, "Successfully registered")
            : new ResponseMessage(404, "Some error occured");
    }

    [Authorize]
    [Get("AddMission")]
    public ResponseMessage AddMission()
    {
        var page = StaticFilesHandler.FetchStaticFile("addMission.html");
        return page != ""
            ? new ResponseMessage(200, page)
            : new ResponseMessage(404, "Couldn't get mission adding page");
    }

    [Authorize]
    [Post("AddMission")]
    public ResponseMessage AddMission(
        string title,
        string continent,
        string country,
        string city,
        string description,
        string url
    )
    {
        var missionReceived = new MissionAddingDto
        {
            Title = title,
            Continent = continent,
            Country = country,
            City = city,
            Description = description,
            Url = url
        };

        var mission = MissionMapper.MapMissionAddingDtoToMission(missionReceived);
        var missionDao = new MissionDao();
        var userIdTuple = _cookiesHandlerService
            .FetchTokenFromCookiesAndGetUserId(_context, _authService, new UserDao());
        
        if (!userIdTuple.Item1)
            return new ResponseMessage(404, "Something went wrong");

        mission.UserId = userIdTuple.Item2;
        
        return missionDao.CreateMission(mission)
            ? new ResponseMessage(200, "Mission was successfully added")
            : new ResponseMessage(404, "Something went wrong");
    }

    [Authorize]
    [Post("DeleteMission")]
    public ResponseMessage DeleteMission(int id)
    {
        var missionDao = new MissionDao();
        
        var tokenTuple = _cookiesHandlerService.FetchTokenFromCookies(_context);
        if (!tokenTuple.Item1)
            return new ResponseMessage(404, "Something went wrong");
        
        if (!_authService.ValidateToken(new UserDao(), tokenTuple.Item2))
            return new ResponseMessage(403, "You do not have access for that");
        
        var mission = missionDao.DeleteMission(id);
        return new ResponseMessage(200, mission?.Title ?? "");
    }

    [Authorize]
    [Get("AddScreenshot")]
    public ResponseMessage AddScreenshot()
    {
        var page = StaticFilesHandler.FetchStaticFile("addScreenshot.html");
        return page != ""
            ? new ResponseMessage(200, page)
            : new ResponseMessage(404, "Couldn't get screenshot adding page");
    }

    [Authorize]
    [Post("AddScreenshot")]
    public ResponseMessage AddScreenshot(
        string text,
        string url)
    {
        var screenshotReceived = new ScreenshotCardAddingDto
        {
            Text = text,
            Url = url
        };

        var screenshotCard = ScreenshotCardMapper
            .MapScreenshotCardAddingDtoToScreenshotCard(screenshotReceived);
        var screenshotCardDao = new ScreenshotCardDao();
        var userIdTuple = _cookiesHandlerService
            .FetchTokenFromCookiesAndGetUserId(_context, _authService, new UserDao());
        
        if (!userIdTuple.Item1)
            return new ResponseMessage(404, "Something went wrong");

        screenshotCard.UserId = userIdTuple.Item2;
        
        return screenshotCardDao.CreateScreenshotCard(screenshotCard)
            ? new ResponseMessage(200, "Screenshot was successfully added")
            : new ResponseMessage(404, "Something went wrong");
    }

    [Authorize]
    [Post("DeleteScreenshot")]
    public ResponseMessage DeleteScreenshot(int id)
    {
        var screenshotCardDao = new ScreenshotCardDao();
        
        var tokenTuple = _cookiesHandlerService.FetchTokenFromCookies(_context);
        if (!tokenTuple.Item1)
            return new ResponseMessage(404, "Something went wrong");
        
        if (!_authService.ValidateToken(new UserDao(), tokenTuple.Item2))
            return new ResponseMessage(403, "You do not have access for that");
        
        var screenshotCard = screenshotCardDao.DeleteScreenshotCard(id);
        return new ResponseMessage(200, screenshotCard?.Text ?? "");
    }

    [Authorize]
    [Get("AddVideo")]
    public ResponseMessage AddVideo()
    {
        var page = StaticFilesHandler.FetchStaticFile("addVideo.html");
        return page != ""
            ? new ResponseMessage(200, page)
            : new ResponseMessage(404, "Couldn't get video adding page");
    }

    [Authorize]
    [Post("AddVideo")]
    public ResponseMessage AddVideo(
        string text,
        string preview,
        string redirect)
    {
        var receivedVideo = new VideoCardAddingDto
        {
            Text = text,
            PreviewUrl = preview,
            RedirectUrl = redirect
        };

        var videoCard = VideoCardMapper.MapVideoCardAddingDtoToVideoCard(receivedVideo);
        var videoCardDao = new VideoCardDao();
        
        var userIdTuple = _cookiesHandlerService
            .FetchTokenFromCookiesAndGetUserId(_context, _authService, new UserDao());
        
        if (!userIdTuple.Item1)
            return new ResponseMessage(404, "Something went wrong");

        videoCard.UserId = userIdTuple.Item2;
        
        return videoCardDao.CreateVideoCard(videoCard)
            ? new ResponseMessage(200, "Screenshot was successfully added")
            : new ResponseMessage(404, "Something went wrong");
    }

    [Authorize]
    [Post("DeleteVideo")]
    public ResponseMessage DeleteVideo(int id)
    {
        var videoCardDao = new VideoCardDao();

        var tokenTuple = _cookiesHandlerService.FetchTokenFromCookies(_context);
        if (!tokenTuple.Item1)
            return new ResponseMessage(404, "Something went wrong");
        
        if (!_authService.ValidateToken(new UserDao(), tokenTuple.Item2))
            return new ResponseMessage(403, "You do not have access for that");
        
        var videoCard = videoCardDao.DeleteVideoCard(id);
        return new ResponseMessage(200, videoCard?.Text ?? "");
    }
}