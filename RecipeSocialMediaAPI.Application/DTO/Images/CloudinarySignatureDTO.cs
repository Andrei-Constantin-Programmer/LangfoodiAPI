namespace RecipeSocialMediaAPI.Application.DTO.ImageHosting;
public record CloudinarySignatureDTO
{
    required public string Signature { get; set; }
    required public long TimeStamp { get; set; }
}
