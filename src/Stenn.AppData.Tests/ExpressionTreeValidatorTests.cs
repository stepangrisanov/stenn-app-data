using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stenn.AppData.Tests
{
    [TestFixture]
    public class ExpressionTreeValidatorTests
    {
        private IQueryable<object> source = new List<object> { 1, 2, 3 }.AsQueryable();

        [Test]
        public void ToStringAllowedTest()
        {
            ExpressionTreeValidator validator = new ExpressionTreeValidator();
            var expression = source.Select(x => x.ToString()).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(ToString), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryDisallowedTest()
        {
            ExpressionTreeValidator validator = new ExpressionTreeValidator();
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeFalse();
        }

        [Test]
        public void GetCurrentDirectoryAllowedExplicitlyTest()
        {
            ExpressionTreeValidator validator = new ExpressionTreeValidator((MethodInfo mi) => mi.Name == nameof(Directory.GetCurrentDirectory));
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryAllowedByModuleTest()
        {
            ExpressionTreeValidator validator = new ExpressionTreeValidator((MethodInfo mi) => mi.Module.Name == typeof(Directory).Methods().First().Module.Name);
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryDisallowedIfAllowedSomethingElseTest()
        {
            ExpressionTreeValidator validator = new ExpressionTreeValidator((MethodInfo mi) => mi.Module.Name == nameof(Directory.CreateDirectory));
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeFalse();
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
