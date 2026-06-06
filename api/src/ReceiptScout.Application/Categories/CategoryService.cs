using ReceiptScout.Application.Categories.Dtos;
using ReceiptScout.Application.Common.Exceptions;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Exceptions;

namespace ReceiptScout.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CategoryService(ICategoryRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryDto dto)
    {
        EnsureAdmin();
        if (await _repository.ExistsByNameAsync(dto.Name))
            throw new InvalidOperationException($"Category '{dto.Name}' already exists.");

        var category = new Category(dto.Name, dto.BasAccount, dto.VatRate);
        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();
        return Map(category);
    }

    public async Task<CategoryResponse> GetByIdAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Category), id);
        return Map(category);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(Map).ToList();
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        EnsureAdmin();
        var category = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Category), id);
        category.UpdateDetails(dto.Name, dto.BasAccount, dto.VatRate);
        _repository.Update(category);
        await _repository.SaveChangesAsync();
        return Map(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        EnsureAdmin();
        var category = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Category), id);
        _repository.Remove(category);
        await _repository.SaveChangesAsync();
    }

    private void EnsureAdmin()
    {
        if (!_currentUser.IsAdmin) throw new ForbiddenException();
    }

    private static CategoryResponse Map(Category c) =>
        new(c.Id, c.Name, c.Slug, c.BasAccount, c.VatRate);
}
