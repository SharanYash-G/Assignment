using MediatR;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    /// <summary>
    /// Handles the removal of a specific user from a procedure in a plan.
    /// </summary>
    public class RemoveUserHandler : IRequestHandler<RemoveUser, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        /// <summary>
        /// Initializes a new instance with the database context.
        /// </summary>
        public RemoveUserHandler(RLContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Removes a specific user assigned to the given plan and procedure.
        /// </summary>
        public async Task<ApiResponse<Unit>> Handle(RemoveUser request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = request.PlanId;
                var procedure = request.ProcedureId;
                var user = request.UserId;

                if (plan <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid Plan ID: {plan}. Must be greater than zero."));

                if (procedure <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid Procedure ID: {procedure}. Must be greater than zero."));

                if (user <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid User ID: {user}. Must be greater than zero."));

                var link = await _context.PlanProcedureUsers.FindAsync(
                    new object[] { plan, procedure, user },
                    cancellationToken);

                if (link is null)
                {
                    var msg = $"No mapping found for PlanId={plan}, ProcedureId={procedure}, UserId={user}";
                    return ApiResponse<Unit>.Fail(new NotFoundException(msg));
                }

                _context.PlanProcedureUsers.Remove(link);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<Unit>.Succeed(Unit.Value);
            }
            catch (Exception ex)
            {
                return ApiResponse<Unit>.Fail(ex);
            }
        }
    }
}
