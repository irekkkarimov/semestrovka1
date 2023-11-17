using semestrovka.DAOs;
using semestrovka.Handlers;
using semestrovka.Models;

namespace semestrovka.TemplateEngine;

public class PageMapper
{
    /// <summary>
    /// Maps Admin Page with elements belonging to user
    /// </summary>
    /// <param name="userId">Data belonging to User with userId will be mapped</param>
    /// <returns>Ready html page in string format</returns>
    public static string MapAdminPage(int userId)
    {
        // Fetching template
        var adminPageTemplate = StaticFilesHandler.FetchStaticFile("admin.html");
        
        // Mapping missions
        var missionTemplate = StaticFilesHandler.FetchStaticFile("components/adminPageMission.html");
        var missionsInOneHtml = MapMissionsForHomePage(missionTemplate);
        
        // Mapping screenshot cards
        var screenshotCardTemplate = StaticFilesHandler.FetchStaticFile("components/adminPageScreenshot.html");
        var screenshotCardsInOneHtml = MapScreenshotCardsForHomePage(screenshotCardTemplate);
        
        // Mapping video cards
        var videoCardTemplate = StaticFilesHandler.FetchStaticFile("components/adminPageVideo.html");
        var videoCardsInOneHtml = MapVideoCardsForHomePage(videoCardTemplate);
        
        // Replacing lists spaces with actual data
        var missionListReplaced = adminPageTemplate.Replace("MISSION_LIST_TR", missionsInOneHtml);
        var screenshotCardsReplaced = missionListReplaced.Replace("SCREENSHOT_LIST", screenshotCardsInOneHtml);
        var videoCardsReplaced = screenshotCardsReplaced.Replace("VIDEO_LIST", videoCardsInOneHtml);
        
        // Returning ready html element
        return videoCardsReplaced;
    }
    
    /// <summary>
    /// Maps home page
    /// </summary>
    /// <returns>Ready html page in string format</returns>
    public static string MapHomePage()
    {
        // Fetching template
        var homePageTemplate = StaticFilesHandler.FetchStaticFile("index.html");
     
        // Mapping missions
        var missionTemplate = StaticFilesHandler.FetchStaticFile("components/mission.html");
        var missionsInOneHtml = MapMissionsForHomePage(missionTemplate);
        
        // Mapping screenshot cards
        var screenshotCardTemplate = StaticFilesHandler.FetchStaticFile("components/screenshot.html");
        var screenshotCardsInOneHtml = MapScreenshotCardsForHomePage(screenshotCardTemplate);
        
        // Mapping video cards
        var videoCardTemplate = StaticFilesHandler.FetchStaticFile("components/video.html");
        var videoCardsInOneHtml = MapVideoCardsForHomePage(videoCardTemplate);
        
        // Replacing lists spaces with actual data
        var missionListReplaced = homePageTemplate.Replace("MISSION_LIST_TR", missionsInOneHtml);
        var screenshotCardsReplaced = missionListReplaced.Replace("SCREENSHOT_LIST", screenshotCardsInOneHtml);
        var videoCardsReplaced = screenshotCardsReplaced.Replace("VIDEO_LIST", videoCardsInOneHtml);
        
        // Returning ready html element
        return videoCardsReplaced;
    }
    
    /// <summary>
    /// Maps Mission Page
    /// </summary>
    /// <param name="mission">Mission to be mapped</param>
    /// <returns>Ready html element in string format</returns>
    public static string MapMissionPage(Mission mission)
    {
        // Fetching template
        var missionPageTemplate = StaticFilesHandler.FetchStaticFile("MissionPage.html");
        
        // Replacing 'key-words' to actual values
        var replacedTitle = missionPageTemplate.Replace("MISSION_TITLE", mission.Title);
        var replacedDescription = replacedTitle.Replace("MISSION_DESCRIPTION", mission.Description);
        var replacedImageUrl = replacedDescription.Replace("MISSION_IMAGE", mission.Url);
        
        // Returning ready html element
        return replacedImageUrl;
    }

    private static string MapMissionsForHomePage(string missionTemplate)
    {
        // Mapping missions
        var missionDao = new MissionDao();
        var missionsList = missionDao.GetAllMissions();
        var missionsMapped = missionsList
            .Select(i => ComponentMapper.MapMission(missionTemplate, i))
            .ToArray();
        
        // Joining missions into one string
        var missionsIntoOneHtml = string.Join("", missionsMapped);
        
        return missionsIntoOneHtml;
    }
    
    private static string MapScreenshotCardsForHomePage(string screenshotCardTemplate)
    {
        // Mapping screenshot cards
        var screenshotCardDao = new ScreenshotCardDao();
        var screenshotCardsList = screenshotCardDao.GetAllScreenshotCards();
        var screenshotCardsMapped = screenshotCardsList
            .Select(i => ComponentMapper.MapScreenshotCard(screenshotCardTemplate, i))
            .ToArray();
        
        // Joining screenshotCards into one string
        var screenshotCardsIntoOneHtml = string.Join("", screenshotCardsMapped);
        
        return screenshotCardsIntoOneHtml;
    }
    
    private static string MapVideoCardsForHomePage(string videoCardTemplate)
    {
        // Mapping video cards
        var videoCardDao = new VideoCardDao();
        var videoCardsList = videoCardDao.GetAllVideoCards();
        var videoCardsMapped = videoCardsList
            .Select(i => ComponentMapper.MapVideoCard(videoCardTemplate, i))
            .ToArray();
        
        // Joining video cards into one string
        var videoCardsIntoOneHtml = string.Join("", videoCardsMapped);
        
        return videoCardsIntoOneHtml;
    }
}