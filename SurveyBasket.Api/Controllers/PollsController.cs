using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Api.Contracts.Polls;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet(template:"")] // OR     [Route(template:"")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetAllAsync(cancellationToken));
    }

    [HttpGet(template: "current")] // OR     
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetCurrentAsync(cancellationToken));
    }

    [HttpGet(template: "{id}")]

    public async Task<IActionResult> GetByID(int id, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.GetAsync(id,cancellationToken);

        return pollResult.IsSuccess ? Ok(pollResult.Value)
            : pollResult.ToProblem() ; //NotFound(pollResult.Error);
    }

    [HttpPost(template: "")]

    public async Task<IActionResult> Create([FromBody]PollRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.CreateAsync(request,cancellationToken);
        return result.IsFailure 
            ? result.ToProblem()
            : CreatedAtAction(nameof(GetByID), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut(template: "{id}")]

    public async Task<IActionResult> Update(int id,PollRequest request,CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request,cancellationToken);
        return result.IsSuccess ? NoContent() 
            : result.ToProblem();

    }


    [HttpDelete(template: "{id}")]

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id,cancellationToken);
        return result.IsSuccess ? NoContent() 
            : result.ToProblem();

    }

    [HttpPut(template: "{id}/togglepublish")]

    public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() 
            : result.ToProblem();
    }
}
