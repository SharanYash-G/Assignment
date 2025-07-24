using FluentAssertions;
using global::RL.Backend.Commands.Handlers.PlanProcedureUsers;
using global::RL.Backend.Commands.PlanProcedureUsers;
using global::RL.Data.DataModels;

namespace RL.Backend.UnitTests.PlanProcedureUsers
{
    /// <summary>
    /// Unit tests for removing all users assigned to a procedure in a plan.
    /// </summary>
    [TestClass]
    public class RemoveAllUsersTests
    {
        /// <summary>
        /// Should return success when there are no users to remove.
        /// </summary>
        [TestMethod]
        public async Task NoUsersToRemove_ReturnsSuccess()
        {
            var db = DbContextHelper.CreateContext();
            var handler = new RemoveAllUsersHandler(db);

            var command = new RemoveAllUsers
            {
                PlanId = 1,
                ProcedureId = 1
            };

            var result = await handler.Handle(command, default);

            result.Succeeded.Should().BeTrue();
        }

        /// <summary>
        /// Should remove all users assigned to the procedure.
        /// </summary>
        [TestMethod]
        public async Task UsersExist_RemovesAll()
        {
            var db = DbContextHelper.CreateContext();
            var handler = new RemoveAllUsersHandler(db);

            db.PlanProcedureUsers.AddRange(new[]
            {
                new PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 1 },
                new PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 2 }
            });

            await db.SaveChangesAsync();

            var command = new RemoveAllUsers
            {
                PlanId = 1,
                ProcedureId = 1
            };

            var result = await handler.Handle(command, default);

            result.Succeeded.Should().BeTrue();
            db.PlanProcedureUsers.Should().BeEmpty("all assigned users should be removed");
        }
    }
}
