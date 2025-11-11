using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Service.Interfaces;

public interface IUserService
{
    Task<UserVm> CreateAsync(CreateUserDto dto, CancellationToken ct);
    Task<UserVm?> GetAsync(Guid id, CancellationToken ct);
    Task<PagedResult<UserVm>> ListAsync(int page, int size, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}