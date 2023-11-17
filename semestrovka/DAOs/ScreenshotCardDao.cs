using semestrovka.Models;
using semestrovka.utils;

namespace semestrovka.DAOs;

public class ScreenshotCardDao : IScreenshotCardDao
{
    public bool CreateScreenshotCard(ScreenshotCard screenshotCard)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Add(screenshotCard);
    }

    public List<ScreenshotCard> GetAllScreenshotCards()
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Select<ScreenshotCard>();
    }

    public ScreenshotCard? GetScreenshotCardById(int id)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.SelectById<ScreenshotCard>(id);
    }

    public List<ScreenshotCard> GetAllScreenshotCardsByUserId(int userId)
    {
        var dbContext = ServerData.Instance().DbContext;
        var screenshotCards = dbContext.Select<ScreenshotCard>();

        return screenshotCards
            .Where(i => i.UserId == userId)
            .ToList();
    }

    public ScreenshotCard? DeleteScreenshotCard(int screenshotCardId)
    {
        var dbContext = ServerData.Instance().DbContext;
        var screenshotCard = dbContext.SelectById<ScreenshotCard>(screenshotCardId);
        if (screenshotCard == null)
            return null;
        dbContext.Delete(screenshotCard);
        return screenshotCard;
    }
}