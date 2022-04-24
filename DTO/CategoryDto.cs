namespace Sc3S.DTO;
public class CategoryDto : BaseDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetCategoryDto> AssetCategories { get; set; } = new();
    public virtual List<CategorySituationDto> CategorySituations { get; set; } = new();
    public virtual List<CommunicateCategoryDto> CommunicateCategories { get; set; } = new();
}
