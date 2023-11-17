using semestrovka.DTOs;
using semestrovka.Models;

namespace semestrovka.Mapper;

public class ScreenshotCardMapper
{
    public static ScreenshotCard MapScreenshotCardAddingDtoToScreenshotCard(ScreenshotCardAddingDto screenshotCardAddingDto)
    {
        return new ScreenshotCard
        {
            Id = 0,
            Text = screenshotCardAddingDto.Text,
            Url = screenshotCardAddingDto.Url,
            UserId = screenshotCardAddingDto.UserId
        };
    }
}