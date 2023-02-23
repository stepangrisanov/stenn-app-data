using FluentAssertions;
using NUnit.Framework;
using System.Linq.Expressions;
using Stenn.AppData.Expressions;
using Stenn.AppData.Server;
using Stenn.TestModel.Domain.AppService.Tests;

namespace Stenn.AppData.Tests
{
    [TestFixture]
    public class ExpressionTreeValidatorTests
    {
        private static readonly IQueryable<object> Source = new object[] { 1, 2, 3 }.AsQueryable();

        [Test]
        public void ToStringAllowedTest()
        {
            var validator = new ExpressionTreeValidator<ITestModelEntity>();
            var expression = Source.Select(x => x.ToString()).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(ToString), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryDisallowedTest()
        {
            var validator = new ExpressionTreeValidator<ITestModelEntity>();
            var expression = Source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            Action act = () => validator.Visit(expression);
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("Expression contains not allowed method"));
        }

        [Test]
        public void GetCurrentDirectoryAllowedExplicitlyTest()
        {
            var validator = new CustomExpressionTreeValidator<ITestModelEntity>(mi => mi.Name == nameof(Directory.GetCurrentDirectory));
            var expression = Source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryAllowedByModuleTest()
        {
            var validator = new CustomExpressionTreeValidator<ITestModelEntity>(mi => mi.Module.Name == typeof(Directory).Methods().First().Module.Name);
            var expression = Source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryDisallowedIfAllowedSomethingElseTest()
        {
            var validator = new CustomExpressionTreeValidator<ITestModelEntity>(mi => mi.Module.Name == nameof(Directory.CreateDirectory));
            var expression = Source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            Action act = () => validator.Visit(expression);
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("Expression contains not allowed method"));
        }
    }

    internal class ExpressionTreeSearch : ExpressionVisitor
    {
        private string _methodName;
        private bool _found;

        public bool FindUsage(string methodName, Expression expression)
        {
            _found = false;
            _methodName = methodName;
            Visit(expression);
            return _found;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (method.Name == _methodName)
            {
                _found = true;
            }

            return base.VisitMethodCall(node);
        }
    }
}