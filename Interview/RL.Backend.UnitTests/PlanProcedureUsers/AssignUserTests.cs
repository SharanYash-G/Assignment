using FluentAssertions;
using global::RL.Backend.Commands.Handlers.PlanProcedureUsers;
using global::RL.Backend.Commands.PlanProcedureUsers;
using global::RL.Backend.Exceptions;
using global::RL.Data.DataModels;
using MediatR;

namespace RL.Backend.UnitTests.PlanProcedureUsers
{
    /// <summary>
    /// Unit tests for assigning users to procedures within a plan.
    /// </summary>
    [TestClass]
    public class AssignUserTests
    {
        /// <summary>
        /// Should return bad request when PlanId is invalid.
        /// </summary>
        [TestMethod]
        public async Task InvalidInputs_ReturnsBadRequest()
        {
            var db = DbContextHelper.CreateContext();
            var handler = new AssignUserHandler(db);

            var invalidCommand = new AssignUserToProcedure
            {
                PlanId = -1,
                ProcedureId = 1,
                UserId = 1
            };

            var result = await handler.Handle(invalidCommand, default);

            result.Succeeded.Should().BeFalse("PlanId is invalid");
            result.Exception.Should().BeOfType<BadRequestException>("should throw BadRequestException for invalid input");
        }

        /// <summary>
        /// Should succeed without duplicating assignment if already assigned.
        /// </summary>
        [TestMethod]
        public async Task AlreadyAssigned_ReturnsSuccessWithoutDuplicate()
        {
            var db = DbContextHelper.CreateContext();
            var planId = 1;
            var procedureId = 2;
            var userId = 3;

            db.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            });

            await db.SaveChangesAsync();

            var handler = new AssignUserHandler(db);

            var command = new AssignUserToProcedure
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            var result = await handler.Handle(command, default);

            result.Succeeded.Should().BeTrue();
            result.Value.Should().BeOfType<Unit>();

            var count = db.PlanProcedureUsers.Count(p =>
                p.PlanId == planId &&
                p.ProcedureId == procedureId &&
                p.UserId == userId);

            count.Should().Be(1, "no duplicate should be created");
        }

        /// <summary>
        /// Should assign a user to procedure when inputs are valid.
        /// </summary>
        [TestMethod]
        public async Task ValidAssignment_AddsToDb()
        {
            var db = DbContextHelper.CreateContext();
            var handler = new AssignUserHandler(db);

            var command = new AssignUserToProcedure
            {
                PlanId = 1,
                ProcedureId = 1,
                UserId = 1
            };

            var result = await handler.Handle(command, default);

            result.Succeeded.Should().BeTrue();
            db.PlanProcedureUsers.Should().ContainSingle();
        }
    }
}
