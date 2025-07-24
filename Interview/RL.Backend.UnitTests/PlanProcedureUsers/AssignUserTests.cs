using FluentAssertions;
using global::RL.Backend.Commands.Handlers.PlanProcedureUsers;
using global::RL.Backend.Commands.PlanProcedureUsers;
using global::RL.Backend.Exceptions;
using global::RL.Data.DataModels;
using MediatR;


namespace RL.Backend.UnitTests.PlanProcedureUsers
{
    [TestClass]
    public class AssignUserTests
    {
        [TestMethod]
        public async Task InvalidInputs_ReturnsBadRequest()
        {
            var context = DbContextHelper.CreateContext();
            var sut = new AssignUserHandler(context);

            var result = await sut.Handle(new AssignUserToProcedure
            {
                PlanId = -1,
                ProcedureId = 1,
                UserId = 1
            }, default);

            result.Succeeded.Should().BeFalse();
            result.Exception.Should().BeOfType<BadRequestException>();
        }

        [TestMethod]
        public async Task AlreadyAssigned_ReturnsSuccessWithoutDuplicate()
        {
            var context = DbContextHelper.CreateContext();
            var planId = 1; var procedureId = 2; var userId = 3;

            context.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            });
            await context.SaveChangesAsync();

            var sut = new AssignUserHandler(context);
            var result = await sut.Handle(new AssignUserToProcedure
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            }, default);

            result.Succeeded.Should().BeTrue();
            result.Value.Should().BeOfType<Unit>();

            var count = context.PlanProcedureUsers
                .Count(p => p.PlanId == planId && p.ProcedureId == procedureId && p.UserId == userId);
            count.Should().Be(1);
        }

        [TestMethod]
        public async Task ValidAssignment_AddsToDb()
        {
            var context = DbContextHelper.CreateContext();
            var sut = new AssignUserHandler(context);

            var result = await sut.Handle(new AssignUserToProcedure
            {
                PlanId = 1,
                ProcedureId = 1,
                UserId = 1
            }, default);

            result.Succeeded.Should().BeTrue();
            context.PlanProcedureUsers.Should().ContainSingle();
        }
    }
}
