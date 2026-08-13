using System.Collections.Generic;

namespace KadirPortfolio.Api.Dtos
{
    public class UpdateAboutSettingDto
    {
        public string MainTitle { get; set; } = string.Empty;
        public string? MainTitleEn { get; set; }
        public string SubTitle { get; set; } = string.Empty;
        public string? SubTitleEn { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string CardTitle { get; set; } = string.Empty;
        public string? CardTitleEn { get; set; }
        public string CardSubtitle { get; set; } = string.Empty;
        public string? CardSubtitleEn { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string? BioEn { get; set; }
        public bool IsLookingForJob { get; set; }

        public List<UpdateAboutCardDto> Cards { get; set; } = new();
    }

    public class UpdateAboutCardDto
    {
        public int Id { get; set; }
        public int CardType { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? TitleEn { get; set; }
        public string? Text { get; set; }
        public string? TextEn { get; set; }
        public List<string>? ListItems { get; set; } 
        public List<string>? ListItemsEn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
