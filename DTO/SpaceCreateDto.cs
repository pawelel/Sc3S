using Sc3S.Enumerations;
namespace Sc3S.DTO;
public class SpaceCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SpaceType SpaceType { get; set; }
}
