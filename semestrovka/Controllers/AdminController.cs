using System.Net;
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
    [Authorize]
    [Get("Index")]
    public static ResponseMessage Index(HttpListenerContext context)
    {
        var cookie = context.Request.Cookies.First(i => i.Name == "Authorization");
        var users = new UserDao().GetUsers().Select(i => new { i.Id, token = i.Email + i.Password});
        var userId = users.First(i => i.token == cookie.Value).Id;
        var mappedPage = PageMapper.MapAdminPage(userId);
        return mappedPage != ""
            ? new ResponseMessage(200, mappedPage)
            : new ResponseMessage(404, "Couldn't load admin page");
    }

    [Get("Login")]
    public static ResponseMessage Login()
    {
        var page = StaticFilesHandler.FetchStaticFile("login.html");
        return new ResponseMessage(200, page);
    }
    
    [Post("Login")]
    public static ResponseMessage Login(string email, string password)
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
    public static ResponseMessage Logout()
    {
        return new ResponseMessage(200, "setcookie Authorization _");
    }
    
    [Get("Registration")]
    public static ResponseMessage Registration()
    {
        var page = StaticFilesHandler.FetchStaticFile("registration.html");
        return new ResponseMessage(200, page);
    }
    
    [Post("Registration")]
    public static ResponseMessage Registration(string email, string password)
    {
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
    public static ResponseMessage AddMission()
    {
        var page = StaticFilesHandler.FetchStaticFile("addMission.html");
        return page != ""
            ? new ResponseMessage(200, page)
            : new ResponseMessage(404, "Couldn't get mission adding page");
    }
    
    [Authorize]
    [Post("AddMission")]
    public static ResponseMessage AddMission(
        string title,
        string continent,
        string country,
        string city,
        string description,
        string url,
        int userId
    )
    {
        var missionReceived = new MissionAddingDto
        {
            UserId = userId,
            Title = title,
            Continent = continent,
            Country = country,
            City = city,
            Description = description,
            Url = url
        };
        
        var mission = MissionMapper.MapMissionAddingDtoToMission(missionReceived);
        var missionDao = new MissionDao();
        return missionDao.CreateMission(mission)
            ? new ResponseMessage(200, "Mission was successfully added")
            : new ResponseMessage(404, "Something went wrong");
    }

    [Authorize]
    [Post("DeleteMission")]
    public static ResponseMessage DeleteMission(int id)
    {
        var missionDao = new MissionDao();
        var mission = missionDao.DeleteMission(id);
        return new ResponseMessage(200, mission?.Title ?? "");
    }

    [Authorize]
    [Get("AddScreenshot")]
    public static ResponseMessage AddScreenshot()
    {
        var page = StaticFilesHandler.FetchStaticFile("addScreenshot.html");
        return page != ""
            ? new ResponseMessage(200, page)
            : new ResponseMessage(404, "Couldn't get screenshot adding page");
    }

    [Authorize]
    [Post("AddScreenshot")]
    public static ResponseMessage AddScreenshot(
        string text,
        string url,
        int userId)
    {
        var screenshotReceived = new ScreenshotCardAddingDto
        {
            Text = text,
            Url = url,
            UserId = userId
        };

        var screenshotCard = ScreenshotCardMapper
            .MapScreenshotCardAddingDtoToScreenshotCard(screenshotReceived);
        var screenshotCardDao = new ScreenshotCardDao();
        return screenshotCardDao.CreateScreenshotCard(screenshotCard)
            ? new ResponseMessage(200, "Screenshot was successfully added")
            : new ResponseMessage(404, "Something went wrong");
    }

    [Authorize]
    [Post("DeleteScreenshot")]
    public static ResponseMessage DeleteScreenshot(int id)
    {
        var screenshotCardDao = new ScreenshotCardDao();
        var screenshotCard = screenshotCardDao.DeleteScreenshotCard(id);
        return new ResponseMessage(200, screenshotCard?.Text ?? "");
    }
    
    [Authorize]
    [Get("AddVideo")]
    public static ResponseMessage AddVideo()
    {
        var page = StaticFilesHandler.FetchStaticFile("addVideo.html");
        return page != ""
            ? new ResponseMessage(200, page)
            : new ResponseMessage(404, "Couldn't get video adding page");
    }

    [Authorize]
    [Post("AddVideo")]
    public static ResponseMessage AddVideo(
        string text,
        string preview,
        string redirect,
        int userId)
    {
        var receivedVideo = new VideoCardAddingDto
        {
            Text = text,
            PreviewUrl = preview,
            RedirectUrl = redirect,
            UserId = userId
        };

        var videoCard = VideoCardMapper.MapVideoCardAddingDtoToVideoCard(receivedVideo);
        var videoCardDao = new VideoCardDao();
        return videoCardDao.CreateVideoCard(videoCard)
            ? new ResponseMessage(200, "Screenshot was successfully added")
            : new ResponseMessage(404, "Something went wrong");
    }

    [Authorize]
    [Post("DeleteVideo")]
    public static ResponseMessage DeleteVideo(int id)
    {
        var videoCardDao = new VideoCardDao();
        var videoCard = videoCardDao.DeleteVideoCard(id);
        return new ResponseMessage(200, videoCard?.Text ?? "");
    }
}