using FluentAssertions;
using global::RL.Backend.Commands.Handlers.PlanProcedureUsers;
using global::RL.Backend.Commands.PlanProcedureUsers;
using global::RL.Backend.Exceptions;
using global::RL.Data.DataModels;

namespace RL.Backend.UnitTests.PlanProcedureUsers
{
    [TestClass]
    public class RemoveUserTests
    {
        [TestMethod]
        public async Task UserAssignmentDoesNotExist_ReturnsNotFound()
        {
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveUserHandler(context);

            var result = await sut.Handle(new RemoveUser
            {
                PlanId = 1,
                ProcedureId = 2,
                UserId = 3
            }, default);

            result.Succeeded.Should().BeFalse();
            result.Exception.Should().BeOfType<NotFoundException>();
        }

        [TestMethod]
        public async Task ValidAssignment_RemovesSuccessfully()
        {
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveUserHandler(context);

            context.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = 1,
                ProcedureId = 2,
                UserId = 3
            });
            await context.SaveChangesAsync();

            var result = await sut.Handle(new RemoveUser
            {
                PlanId = 1,
                ProcedureId = 2,
                UserId = 3
            }, default);

            result.Succeeded.Should().BeTrue();
            context.PlanProcedureUsers.Should().BeEmpty();
        }
    }
}
