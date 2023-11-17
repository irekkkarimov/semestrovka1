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

    [Get("MissionPage")]
    public static ResponseMessage MissionPage(int id)
    {
        var missionDao = new MissionDao();
        var mission = missionDao.GetMissionById(id);
        if (mission == null)
            return new ResponseMessage(404, "");
        var mappedPage = PageMapper.MapMissionPage(mission);
        return new ResponseMessage(200, mappedPage);
    }
}