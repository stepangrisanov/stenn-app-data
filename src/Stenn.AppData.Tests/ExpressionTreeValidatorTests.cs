using FluentAssertions;
using NUnit.Framework;
using System;
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
            var validator = new ExpressionTreeValidator<object>();
            var expression = source.Select(x => x.ToString()).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(ToString), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryDisallowedTest()
        {
            var validator = new ExpressionTreeValidator<object>();
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            Action act = () => validator.Visit(expression);
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("Expression contains not allowed method"));
        }

        [Test]
        public void GetCurrentDirectoryAllowedExplicitlyTest()
        {
            var validator = new ExpressionTreeValidator<object>((MethodInfo mi) => mi.Name == nameof(Directory.GetCurrentDirectory));
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryAllowedByModuleTest()
        {
            var validator = new ExpressionTreeValidator<object>((MethodInfo mi) => mi.Module.Name == typeof(Directory).Methods().First().Module.Name);
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
            var sanitizedExpression = validator.Visit(expression);
            new ExpressionTreeSearch().FindUsage(nameof(Directory.GetCurrentDirectory), sanitizedExpression).Should().BeTrue();
        }

        [Test]
        public void GetCurrentDirectoryDisallowedIfAllowedSomethingElseTest()
        {
            var validator = new ExpressionTreeValidator<object>((MethodInfo mi) => mi.Module.Name == nameof(Directory.CreateDirectory));
            var expression = source.Select(x => new { Text = Directory.GetCurrentDirectory() }).Expression;
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
