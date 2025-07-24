using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class AssignUserHandler : IRequestHandler<AssignUserToProcedure, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AssignUserHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(AssignUserToProcedure request, CancellationToken cancellationToken)
        {
            try
            {
                var planId = request.PlanId;
                var procedureId = request.ProcedureId;
                var userId = request.UserId;

                if (planId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Plan ID must be positive. Provided: {planId}"));

                if (procedureId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Procedure ID must be positive. Provided: {procedureId}"));

                if (userId <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"User ID must be positive. Provided: {userId}"));

                var alreadyExists = await _context.PlanProcedureUsers.AnyAsync(entry =>
                    entry.PlanId == planId &&
                    entry.ProcedureId == procedureId &&
                    entry.UserId == userId,
                    cancellationToken);

                if (!alreadyExists)
                {
                    var newMapping = new PlanProcedureUser
                    {
                        PlanId = planId,
                        ProcedureId = procedureId,
                        UserId = userId
                    };

                    _context.PlanProcedureUsers.Add(newMapping);
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
