namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet(template:"")] // OR     [Route(template:"")]
    public IActionResult GetAll()
    {
        return Ok(_pollService.GetAll());
    }

    [HttpGet(template:"{id}")]

    public IActionResult GetByID(int id)
    {
        var poll = _pollService.Get(id);

        return poll is null? NotFound(): Ok(poll);
    }

    [HttpPost(template:"")]

    public IActionResult Create(Poll poll)
    {
        var newPoll = _pollService.Create(poll);
        return newPoll is null ? NotFound() : CreatedAtAction(nameof(GetByID),new {id = newPoll.Id},newPoll );
    }

    [HttpPut(template: "{id}")]

    public IActionResult Update(int id, Poll poll)
    {
        var isUpdated = _pollService.Update(id,poll);
        return isUpdated ? NoContent() : NotFound();
        
    }


    [HttpDelete(template: "{id}")]

    public IActionResult Delete(int id)
    {
        var isDeleted = _pollService.Delete(id);
        return isDeleted ? NoContent() : NotFound();

    }
}
