using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceiptScout.Application.Ai;
using ReceiptScout.Application.Ai.Dtos;
using ReceiptScout.Application.Receipts;
using ReceiptScout.Application.Receipts.Dtos;

namespace ReceiptScout.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _service;
    private readonly IReceiptCategorizationService _categorization;

    public ReceiptsController(IReceiptService service, IReceiptCategorizationService categorization)
    {
        _service = service;
        _categorization = categorization;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReceiptResponse>>> GetAll()
        => Ok(await _service.GetAllForCurrentUserAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReceiptResponse>> GetById(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<ReceiptResponse>> Create(CreateReceiptDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ReceiptResponse>> Update(Guid id, UpdateReceiptDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/suggest-category")]
    public async Task<ActionResult<CategorySuggestion>> SuggestCategory(Guid id)
        => Ok(await _categorization.SuggestForReceiptAsync(id));
}