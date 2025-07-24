using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    /// <summary>
    /// Handles the removal of all users from a procedure in a plan.
    /// </summary>
    public class RemoveAllUsersHandler : IRequestHandler<RemoveAllUsers, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        /// <summary>
        /// Initializes a new instance with the database context.
        /// </summary>
        public RemoveAllUsersHandler(RLContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Removes all users assigned to the specified plan and procedure.
        /// </summary>
        public async Task<ApiResponse<Unit>> Handle(RemoveAllUsers request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = request.PlanId;
                var procedure = request.ProcedureId;

                if (plan <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid Plan ID: {plan}. It must be greater than zero."));

                if (procedure <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid Procedure ID: {procedure}. It must be greater than zero."));

                var userLinks = await _context.PlanProcedureUsers
                    .Where(link => link.PlanId == plan && link.ProcedureId == procedure)
                    .ToListAsync(cancellationToken);

                if (userLinks.Any())
                {
                    _context.PlanProcedureUsers.RemoveRange(userLinks);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return ApiResponse<Unit>.Succeed(Unit.Value);
            }
            catch (Exception ex)
            {
                return ApiResponse<Unit>.Fail(ex);
            }
        }
    }
}
