using SurveyBasket.Api.Contracts.Request;
using SurveyBasket.Api.Contracts.Responses;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet(template:"")] // OR     [Route(template:"")]
    public async Task<IActionResult> GetAll()
    {
        var polls = await _pollService.GetAllAsync();
        var response = polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(response);
    }

    [HttpGet(template: "{id}")]

    public async Task<IActionResult> GetByID(int id)
    {
        var poll = await _pollService.GetAsync(id);

        if (poll is null)
            return NotFound();

        var response = poll.Adapt<PollResponse>();

        return Ok(response);
    }

    [HttpPost(template: "")]

    public async Task<IActionResult> Create([FromBody]CreatePollRequest request)
    {
        var newPoll = await _pollService.CreateAsync(request);
        return newPoll is null ? NotFound() : CreatedAtAction(nameof(GetByID), new { id = newPoll.Id }, newPoll);
    }

    //[HttpPut(template: "{id}")]

    //public IActionResult Update(int id, Poll poll)
    //{
    //    var isUpdated = _pollService.Update(id,poll);
    //    return isUpdated ? NoContent() : NotFound();

    //}


    //[HttpDelete(template: "{id}")]

    //public IActionResult Delete(int id)
    //{
    //    var isDeleted = _pollService.Delete(id);
    //    return isDeleted ? NoContent() : NotFound();

    //}
}
