namespace WebPracticalProject.Service.Dto;

public sealed class CreateContactDto
{
    public Guid? UserId { get; set; }  
    public string? Name { get; init; }
    public required string Email { get; init; }
    public string? Subject { get; init; }
    public required string Message { get; init; }
}
public sealed class ContactVm
{
    public required Guid Id { get; init; }
    public Guid? UserId { get; set; }   
    public string? Name { get; init; }
    public required string Email { get; init; }
    public string? Subject { get; init; }
    public required string Message { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
