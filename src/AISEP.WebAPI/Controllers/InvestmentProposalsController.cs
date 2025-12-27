using AISEP.Application.Features.InvestmentProposals.Commands;
using AISEP.Application.Features.InvestmentProposals.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AISEP.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentProposalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvestmentProposalsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Create a new investment proposal
    /// </summary>
    /// <param name="command">Investment proposal creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created investment proposal</returns>
    [HttpPost]
    [ProducesResponseType(typeof(InvestmentProposalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InvestmentProposalDto>> CreateProposal(
        [FromBody] CreateInvestmentProposalCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(CreateProposal), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
        }
    }
}
