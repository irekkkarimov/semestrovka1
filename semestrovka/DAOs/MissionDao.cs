using semestrovka.Models;
using semestrovka.utils;

namespace semestrovka.DAOs;

public class MissionDao : IMissionDao
{
    public bool CreateMission(Mission mission)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Add(mission);  
    }

    public List<Mission> GetAllMissions()
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Select<Mission>();
    }

    public Mission? GetMissionById(int id)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.SelectById<Mission>(id);
    }

    public List<Mission> GetAllMissionsByUserId(int userId)
    {
        var dbContext = ServerData.Instance().DbContext;
        var missions = dbContext.Select<Mission>();
        
        return missions
            .Where(i => i.UserId == userId)
            .ToList();
    }

    public Mission? DeleteMission(int missionId)
    {
        var dbContext = ServerData.Instance().DbContext;
        var mission = dbContext.SelectById<Mission>(missionId);
        if (mission == null)
            return null;
        dbContext.Delete(mission);
        return mission;
    }
}