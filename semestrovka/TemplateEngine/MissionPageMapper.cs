using semestrovka.Handlers;
using semestrovka.Models;
using semestrovka.utils;

namespace semestrovka.TemplateEngine;

public class MissionPageMapper
{
    public static string Map(Mission mission)
    {
        var missionPageTemplate = StaticFilesHandler.FetchStaticFile("Mission.html");
        var replacedTitle = missionPageTemplate.Replace("MISSION_TITLE", mission.Title);
        var replacedDescription = replacedTitle.Replace("MISSION_DESCRIPTION", mission.Description);
        var replacedImageUrl = replacedDescription.Replace("MISSION_IMAGE", mission.Url);
        return replacedImageUrl;
    }
}