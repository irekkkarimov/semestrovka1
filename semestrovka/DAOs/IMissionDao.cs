using semestrovka.Models;

namespace semestrovka.DAOs;

public interface IMissionDao
{
    public bool CreateMission(Mission mission);
    public Mission GetMissionById(int id);
    public Mission GetMissionByUserId(int userId);
    public Mission DeleteMission(int missionId);
}