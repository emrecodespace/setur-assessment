using AutoFixture;
using AutoFixture.AutoMoq;
using Contacts.Application.Common;
using FluentAssertions;

namespace Contacts.Tests;

public abstract class TestBase
{
    protected readonly IFixture Fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

    protected TestBase()
    {
        Fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    protected static void AssertSuccess<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ErrorMessage.Should().BeNull();
    }

    protected static void AssertFailure<T>(Result<T> result, int expectedStatusCode = 400)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        if (typeof(T).IsValueType)
            result.Value.Should().Be(default(T));
        else
            result.Value.Should().BeNull();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        result.StatusCode.Should().Be(expectedStatusCode);
    }
}