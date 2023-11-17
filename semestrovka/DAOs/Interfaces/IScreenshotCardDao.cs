using semestrovka.Models;

namespace semestrovka.DAOs;

public interface IScreenshotCardDao
{
    bool CreateScreenshotCard(ScreenshotCard screenshotCard);
    List<ScreenshotCard> GetAllScreenshotCards();
    ScreenshotCard? GetScreenshotCardById(int id);
    List<ScreenshotCard> GetAllScreenshotCardsByUserId(int userId);
    ScreenshotCard? DeleteScreenshotCard(int screenshotCardId);
}