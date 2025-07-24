using global::RL.Backend.Commands.PlanProcedureUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers
{
    [ApiController]
   [Route("api/plan-procedures")]
    public class PlanProcedureUsersController : ControllerBase
    {
        private readonly RLContext _context;
        private readonly IMediator _mediator;

        public PlanProcedureUsersController(RLContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AssignUser([FromBody] AssignUserToProcedure command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpDelete("{planId:int}/{procedureId:int}/{userId:int}")]
        public async Task<IActionResult> RemoveUser(int planId, int procedureId, int userId)
        {
            var result = await _mediator.Send(new RemoveUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            });

            return result.ToActionResult();
        }

        [HttpDelete("{planId:int}/{procedureId:int}")]
        public async Task<IActionResult> RemoveAllUsers(int planId, int procedureId)
        {
            var result = await _mediator.Send(new RemoveAllUsers
            {
                PlanId = planId,
                ProcedureId = procedureId
            });

            return result.ToActionResult();
        }

        [HttpGet("{planId:int}/{procedureId:int}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAssignedUsers(int planId, int procedureId)
        {
            if (planId < 1)
                return BadRequest($"Invalid PlanId: {planId}. Must be greater than zero.");

            if (procedureId < 1)
                return BadRequest($"Invalid ProcedureId: {procedureId}. Must be greater than zero.");

            var users = await _context.PlanProcedureUsers
                .Where(pu => pu.PlanId == planId && pu.ProcedureId == procedureId)
                .Include(pu => pu.User)
                .Select(pu => pu.User)
                .ToListAsync();

            return Ok(users);
        }
    }
}
