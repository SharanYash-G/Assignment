using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class RemoveAllUsersHandler : IRequestHandler<RemoveAllUsers, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public RemoveAllUsersHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(RemoveAllUsers request, CancellationToken cancellationToken)
        {
            try
            {
                var planId = request.PlanId;
                var procedureId = request.ProcedureId;

                if (planId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Plan ID must be greater than zero. Received: {planId}"));

                if (procedureId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Procedure ID must be greater than zero. Received: {procedureId}"));

                var assignedUsers = await _context.PlanProcedureUsers
                    .Where(record => record.PlanId == planId && record.ProcedureId == procedureId)
                    .ToListAsync(cancellationToken);

                if (assignedUsers.Any())
                {
                    _context.PlanProcedureUsers.RemoveRange(assignedUsers);
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
