using semestrovka.Models;

namespace semestrovka.DAOs;

public interface IVideoCardDao
{
    bool CreateVideoCard(VideoCard videoCard);
    List<VideoCard> GetAllVideoCards();
    VideoCard? GetVideoCardById(int id);
    List<VideoCard> GetAllVideoCardsByUserId(int userId);
    VideoCard? DeleteVideoCard(int videoCardId);
}