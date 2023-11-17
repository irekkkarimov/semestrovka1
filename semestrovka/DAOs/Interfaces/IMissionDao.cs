using semestrovka.Models;

namespace semestrovka.DAOs;

public interface IMissionDao
{
    public bool CreateMission(Mission mission);
    public List<Mission> GetAllMissions();
    public Mission? GetMissionById(int id);
    public List<Mission> GetAllMissionsByUserId(int userId);
    public Mission? DeleteMission(int id);
}