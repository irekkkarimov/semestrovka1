using semestrovka.Models;
using semestrovka.utils;

namespace semestrovka.DAOs;

public class VideoCardDao : IVideoCardDao
{
    public bool CreateVideoCard(VideoCard videoCard)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Add(videoCard);
    }

    public List<VideoCard> GetAllVideoCards()
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Select<VideoCard>();
    }

    public VideoCard? GetVideoCardById(int id)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.SelectById<VideoCard>(id);
    }

    public List<VideoCard> GetAllVideoCardsByUserId(int userId)
    {
        var dbContext = ServerData.Instance().DbContext;
        var videoCards = dbContext.Select<VideoCard>();

        return videoCards
            .Where(i => i.UserId == userId)
            .ToList();
    }

    public VideoCard? DeleteVideoCard(int videoCardId)   
    {
        var dbContext = ServerData.Instance().DbContext;
        var videoCard = dbContext.SelectById<VideoCard>(videoCardId);
        if (videoCard == null)
            return null;
        dbContext.Delete(videoCard);
        return videoCard;
    }
}