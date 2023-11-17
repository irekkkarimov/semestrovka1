using semestrovka.Models;

namespace semestrovka.TemplateEngine;

public class ComponentMapper
{
    /// <summary>
    /// Maps Mission into html element
    /// </summary>
    /// <param name="template">Html template to map Mission to</param>
    /// <param name="mission">Mission to be mapped</param>
    /// <returns>Ready html element in string format</returns>
    public static string MapMission(string template, Mission mission)
    {
        var titleReplaced = template.Replace("MISSION_TITLE", mission.Title);
        var imageReplaced = titleReplaced.Replace("MISSION_IMAGE", mission.Url);
        var continentReplaced = imageReplaced.Replace("MISSION_CONTINENT", mission.Continent);
        var countryReplaced = continentReplaced.Replace("MISSION_COUNTRY", mission.Country);
        var cityReplaced = countryReplaced.Replace("MISSION_CITY", mission.City);
        var idReplaced = cityReplaced.Replace("MISSION_ID", mission.Id.ToString());
        return idReplaced;
    }

    /// <summary>
    /// Maps Screenshot data into html card element
    /// </summary>
    /// <param name="template">Html template to map Screenshot data to</param>
    /// <param name="screenshotCard">Screenshot data to be mapped</param>
    /// <returns>Ready html element in string format</returns>
    public static string MapScreenshotCard(string template, ScreenshotCard screenshotCard)
    {
        var textReplaced = template.Replace("SCREENSHOT_TEXT", screenshotCard.Text);
        var imageReplaced = textReplaced.Replace("SCREENSHOT_IMAGE", screenshotCard.Url);
        var idReplaced = imageReplaced.Replace("SCREENSHOT_ID", screenshotCard.Id.ToString());
        return idReplaced;
    }

    /// <summary>
    /// Maps Video data into html card element
    /// </summary>
    /// <param name="template">Html template to map Video data to</param>
    /// <param name="videoCard">Video data to be mapped</param>
    /// <returns>Ready html element in string format</returns>
    public static string MapVideoCard(string template, VideoCard videoCard)
    {
        var textReplaced = template.Replace("VIDEO_TEXT", videoCard.Text);
        var previewReplaced = textReplaced.Replace("VIDEO_PREVIEW", videoCard.PreviewUrl);
        var redirectReplaced = previewReplaced.Replace("VIDEO_REDIRECT", videoCard.RedirectUrl);
        var idReplaced = redirectReplaced.Replace("VIDEO_ID", videoCard.Id.ToString());
        return idReplaced;
    }
}