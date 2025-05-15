using Azure.Core;
using SurveyBasket.Api.Contracts.Request;
using SurveyBasket.Api.Contracts.Responses;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet(template:"")] // OR     [Route(template:"")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        var response = polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(response);
    }

    [HttpGet(template: "{id}")]

    public async Task<IActionResult> GetByID(int id, CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetAsync(id,cancellationToken);

        if (poll is null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();

        return Ok(response);
    }

    [HttpPost(template: "")]

    public async Task<IActionResult> Create([FromBody]PollRequest request,
        CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.CreateAsync(request,cancellationToken);
        return newPoll is null ? NotFound() : CreatedAtAction(nameof(GetByID), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut(template: "{id}")]

    public async Task<IActionResult> Update(int id,PollRequest request,CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.UpdateAsync(id, request,cancellationToken);
        return isUpdated ? NoContent() : NotFound();

    }


    [HttpDelete(template: "{id}")]

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id,cancellationToken);
        return isDeleted ? NoContent() : NotFound();

    }

    [HttpPut(template: "{id}/togglepublish")]

    public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
    {
        var status = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        return status ? NoContent() : NotFound();
    }
}
