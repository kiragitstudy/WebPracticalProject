using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Users;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Service.Services;
public sealed class UserService(IUserRepository repo) : IUserService
{
    public async Task<UserVm> CreateAsync(CreateUserDto dto, CancellationToken ct)
    {
        var id = await repo.CreateAsync(new CreateUserArgs(dto.Email.Trim(), dto.Password /*hash тут*/, dto.DisplayName, dto.Role), ct);
        var got = await repo.GetByIdAsync(id, ct);
        return new UserVm { Id=got.Id, Email=got.Email, DisplayName=got.DisplayName, Role=got.Role };
    }
    
    public async Task<UserVm?> GetAsync(Guid id, CancellationToken ct)
    {
        var u = await repo.GetByIdAsync(id, ct);
        return u is null ? null : new UserVm { Id = u.Id, Email = u.Email, DisplayName = u.DisplayName, Role = u.Role };
    }

    public async Task<PagedResult<UserVm>> ListAsync(int page, int size, CancellationToken ct)
    {
        var (items,total) = await repo.GetPagedAsync(page, size, ct);
        return new PagedResult<UserVm>
        {
            Page = page, Size = size, Total = total,
            Items = items.Select(u => new UserVm { Id=u.Id, Email=u.Email, DisplayName=u.DisplayName, Role=u.Role }).ToList()
        };
    }
    
    public async Task UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken ct) =>
        await repo.UpdateAsync(id, new UpdateUserArgs(dto.DisplayName, dto.Role, dto.EmailConfirmed), ct);

    public Task DeleteAsync(Guid id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}