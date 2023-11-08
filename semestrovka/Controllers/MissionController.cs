using semestrovka.Attributes;
using semestrovka.DAOs;
using semestrovka.DTOs;
using semestrovka.Mapper;
using semestrovka.TemplateEngine;
using semestrovka.utils;

namespace semestrovka.Controllers;

[Controller("Mission")]
public class MissionController
{

    [Post("Add")]
    public static ResponseMessage Add(
        string title,
        string continent,
        string country,
        string city,
        string description,
        string url,
        int userid
    )
    {
        var missionReceived = new MissionAddingDto
        {
            UserId = userid,
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

    [Get("MissionPage")]
    public static ResponseMessage MissionPage(int id)
    {
        var missionDao = new MissionDao();
        var mission = missionDao.GetMissionById(id);
        var mappedPage = MissionPageMapper.Map(mission);
        return new ResponseMessage(200, mappedPage);
    }
}