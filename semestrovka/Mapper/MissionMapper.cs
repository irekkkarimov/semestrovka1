using semestrovka.DTOs;
using semestrovka.Models;

namespace semestrovka.Mapper;

public class MissionMapper
{
    public static Mission MapMissionAddingDtoToMission(MissionAddingDto missionAddingDto)
    {
        return new Mission
        {
            Id = 0,
            UserId = missionAddingDto.UserId,
            Title = missionAddingDto.Title,
            Continent = missionAddingDto.Continent,
            Country = missionAddingDto.Country,
            City = missionAddingDto.City,
            Description = missionAddingDto.Description,
            Url = missionAddingDto.Url
        };
    }
}