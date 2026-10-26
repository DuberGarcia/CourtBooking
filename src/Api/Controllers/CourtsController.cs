using Application.Courts.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public sealed record CreateCourtRequest(string Name);

[ApiController]
[Route("api/courts")]
public class CourtsController : ControllerBase
{
    private readonly CreateCourt _createCourt;

    public CourtsController(CreateCourt createCourt)
    {
        _createCourt = createCourt;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourtRequest request)
    {
        var id = await _createCourt.ExecuteAsync(new CreateCourtCommand(request.Name));

        return Created($"/api/courts/{id}", new { id });
    }
}