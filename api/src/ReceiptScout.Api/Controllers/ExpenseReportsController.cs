using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceiptScout.Application.ExpenseReports;
using ReceiptScout.Application.ExpenseReports.Dtos;
using ReceiptScout.Application.Receipts.Dtos;

namespace ReceiptScout.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpenseReportsController : ControllerBase
{
    private readonly IExpenseReportService _service;

    public ExpenseReportsController(IExpenseReportService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExpenseReportResponse>>> GetAll()
        => Ok(await _service.GetAllForCurrentUserAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseReportResponse>> GetById(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpGet("{id:guid}/receipts")]
    public async Task<ActionResult<IReadOnlyList<ReceiptResponse>>> GetReceipts(Guid id)
        => Ok(await _service.GetReceiptsAsync(id));

    [HttpPost]
    public async Task<ActionResult<ExpenseReportResponse>> Create(CreateExpenseReportDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseReportResponse>> Update(Guid id, UpdateExpenseReportDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<ExpenseReportResponse>> Submit(Guid id)
        => Ok(await _service.SubmitAsync(id));

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExpenseReportResponse>> Approve(Guid id)
        => Ok(await _service.ApproveAsync(id));

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ExpenseReportResponse>> Reject(Guid id)
        => Ok(await _service.RejectAsync(id));
}