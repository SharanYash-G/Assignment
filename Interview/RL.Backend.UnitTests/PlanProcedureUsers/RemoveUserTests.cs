using FluentAssertions;
using global::RL.Backend.Commands.Handlers.PlanProcedureUsers;
using global::RL.Backend.Commands.PlanProcedureUsers;
using global::RL.Backend.Exceptions;
using global::RL.Data.DataModels;

namespace RL.Backend.UnitTests.PlanProcedureUsers
{
    /// <summary>
    /// Unit tests for removing a single user from a procedure in a plan.
    /// </summary>
    [TestClass]
    public class RemoveUserTests
    {
        /// <summary>
        /// Should return NotFound when the user-procedure-plan combination does not exist.
        /// </summary>
        [TestMethod]
        public async Task UserAssignmentDoesNotExist_ReturnsNotFound()
        {
            var db = DbContextHelper.CreateContext();
            var handler = new RemoveUserHandler(db);

            var command = new RemoveUser
            {
                PlanId = 1,
                ProcedureId = 2,
                UserId = 3
            };

            var result = await handler.Handle(command, default);

            result.Succeeded.Should().BeFalse();
            result.Exception.Should().BeOfType<NotFoundException>();
        }

        /// <summary>
        /// Should remove the user successfully if the assignment exists.
        /// </summary>
        [TestMethod]
        public async Task ValidAssignment_RemovesSuccessfully()
        {
            var db = DbContextHelper.CreateContext();
            var handler = new RemoveUserHandler(db);

            db.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = 1,
                ProcedureId = 2,
                UserId = 3
            });
            await db.SaveChangesAsync();

            var command = new RemoveUser
            {
                PlanId = 1,
                ProcedureId = 2,
                UserId = 3
            };

            var result = await handler.Handle(command, default);

            result.Succeeded.Should().BeTrue();
            db.PlanProcedureUsers.Should().BeEmpty("the user assignment should be deleted");
        }
    }
}
