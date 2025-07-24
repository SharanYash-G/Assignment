using FluentAssertions;
using global::RL.Backend.Commands.Handlers.PlanProcedureUsers;
using global::RL.Backend.Commands.PlanProcedureUsers;
using global::RL.Data.DataModels;

namespace RL.Backend.UnitTests.PlanProcedureUsers
{
    [TestClass]
    public class RemoveAllUsersTests
    {
        [TestMethod]
        public async Task NoUsersToRemove_ReturnsSuccess()
        {
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveAllUsersHandler(context);

            var result = await sut.Handle(new RemoveAllUsers
            {
                PlanId = 1,
                ProcedureId = 1
            }, default);

            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task UsersExist_RemovesAll()
        {
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveAllUsersHandler(context);

            context.PlanProcedureUsers.AddRange(new[]
            {
            new PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 1 },
            new PlanProcedureUser { PlanId = 1, ProcedureId = 1, UserId = 2 }
        });
            await context.SaveChangesAsync();

            var result = await sut.Handle(new RemoveAllUsers
            {
                PlanId = 1,
                ProcedureId = 1
            }, default);

            result.Succeeded.Should().BeTrue();
            context.PlanProcedureUsers.Should().BeEmpty();
        }
    }
}
