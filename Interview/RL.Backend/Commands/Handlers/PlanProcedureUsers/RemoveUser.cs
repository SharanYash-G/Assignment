using MediatR;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class RemoveUserHandler : IRequestHandler<RemoveUser, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public RemoveUserHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(RemoveUser request, CancellationToken cancellationToken)
        {
            try
            {
                var planId = request.PlanId;
                var procedureId = request.ProcedureId;
                var userId = request.UserId;

                if (planId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Plan ID must be greater than zero. Given: {planId}"));

                if (procedureId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Procedure ID must be greater than zero. Given: {procedureId}"));

                if (userId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"User ID must be greater than zero. Given: {userId}"));

                var mapping = await _context.PlanProcedureUsers.FindAsync(
                    new object[] { planId, procedureId, userId },
                    cancellationToken);

                if (mapping is null)
                {
                    var msg = $"No mapping found for PlanId={planId}, ProcedureId={procedureId}, UserId={userId}";
                    return ApiResponse<Unit>.Fail(new NotFoundException(msg));
                }

                _context.PlanProcedureUsers.Remove(mapping);
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
