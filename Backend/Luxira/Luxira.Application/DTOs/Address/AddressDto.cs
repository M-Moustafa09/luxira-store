namespace Luxira.Application.DTOs.Address;

public class AddressDto
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AddressDetails { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
