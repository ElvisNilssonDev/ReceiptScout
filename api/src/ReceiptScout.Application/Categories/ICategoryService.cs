using ReceiptScout.Application.Categories.Dtos;

namespace ReceiptScout.Application.Categories;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(CreateCategoryDto dto);
    Task<CategoryResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task DeleteAsync(Guid id);
}