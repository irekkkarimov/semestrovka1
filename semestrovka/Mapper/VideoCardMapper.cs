using semestrovka.DTOs;
using semestrovka.Models;

namespace semestrovka.Mapper;

public class VideoCardMapper
{
    public static VideoCard MapVideoCardAddingDtoToVideoCard(VideoCardAddingDto videoCardAddingDto)
    {
        return new VideoCard
        {
            Id = 0,
            Text = videoCardAddingDto.Text,
            PreviewUrl = videoCardAddingDto.PreviewUrl,
            RedirectUrl = videoCardAddingDto.RedirectUrl,
            UserId = videoCardAddingDto.UserId
        };
    }
}