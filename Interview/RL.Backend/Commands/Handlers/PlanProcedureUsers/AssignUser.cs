using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    /// <summary>
    /// Handles assigning a user to a procedure.
    /// </summary>
    public class AssignUserHandler : IRequestHandler<AssignUserToProcedure, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        /// <summary>
        /// Constructor to inject the database context.
        /// </summary>
        public AssignUserHandler(RLContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Assigns a user to a procedure if not already assigned.
        /// </summary>
        public async Task<ApiResponse<Unit>> Handle(AssignUserToProcedure request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = request.PlanId;
                var procedure = request.ProcedureId;
                var user = request.UserId;

                if (user <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid User ID: {user}. It must be greater than 0."));

                if (plan <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid Plan ID: {plan}. It must be greater than 0."));

                if (procedure <= 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid Procedure ID: {procedure}. It must be greater than 0."));

                var exists = await _context.PlanProcedureUsers.AnyAsync(
                    entry => entry.PlanId == plan && entry.ProcedureId == procedure && entry.UserId == user,
                    cancellationToken
                );

                if (!exists)
                {
                    var mapping = new PlanProcedureUser
                    {
                        PlanId = plan,
                        ProcedureId = procedure,
                        UserId = user
                    };

                    _context.PlanProcedureUsers.Add(mapping);
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
