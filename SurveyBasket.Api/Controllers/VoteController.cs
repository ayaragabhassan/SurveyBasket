using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Controllers;
[Route("api/poll/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class VoteController(IVoteService voteService) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;

    //Get All Avilable Questions For Start Voting 
    [HttpGet(template: "")]
    public async Task<IActionResult> StartVote([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId(); 
        var result = await _voteService.GetAvilableAsync(pollId, userId!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }



    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Created() : result.ToProblem();
    }
}
