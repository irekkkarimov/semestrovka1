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

    public Mission GetMissionById(int id)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.SelectById<Mission>(id);
    }

    public Mission GetMissionByUserId(int userId)
    {
        throw new NotImplementedException();
    }

    public Mission DeleteMission(int missionId)
    {
        throw new NotImplementedException();
    }
}