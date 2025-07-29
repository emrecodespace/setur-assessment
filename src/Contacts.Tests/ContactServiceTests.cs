using AutoFixture;
using Contacts.Application.Dtos;
using Contacts.Application.Services;
using Contacts.Domain.Entities;
using Contacts.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Contacts.Tests;

public class ContactServiceTests : TestBase
{
    private readonly Mock<IContactRepository> _repositoryMock;
    private readonly Mock<IValidator<CreatePersonRequest>> _createPersonValidatorMock;
    private readonly Mock<IValidator<AddContactInfoRequest>> _addContactInfoValidatorMock;
    private readonly ContactService _service;

    public ContactServiceTests()
    {
        _repositoryMock = Fixture.Freeze<Mock<IContactRepository>>();
        _createPersonValidatorMock = Fixture.Freeze<Mock<IValidator<CreatePersonRequest>>>();
        _addContactInfoValidatorMock = Fixture.Freeze<Mock<IValidator<AddContactInfoRequest>>>();
        
        _service = new ContactService(
            _repositoryMock.Object, 
            _createPersonValidatorMock.Object, 
            _addContactInfoValidatorMock.Object);
    }

    [Fact]
    public async Task CreatePersonAsync_WithValidRequest_ShouldReturnSuccess()
    {
        var request = Fixture.Create<CreatePersonRequest>();

        _createPersonValidatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        _repositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreatePersonAsync(request);

        AssertSuccess(result);
        result.Value.Should().NotBe(Guid.Empty);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatePersonAsync_WithInvalidRequest_ShouldReturnValidationFailure()
    {
        var request = Fixture.Create<CreatePersonRequest>();
        var validationFailures = new List<ValidationFailure>
        {
            new("FirstName", "First name is required"),
            new("LastName", "Last name is required")
        };
        var validationResult = new ValidationResult(validationFailures);

        _createPersonValidatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var result = await _service.CreatePersonAsync(request);

        AssertFailure(result, 400);
        result.ErrorMessage.Should().Contain("First name is required");
        result.ErrorMessage.Should().Contain("Last name is required");

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeletePersonAsync_WhenPersonExists_ShouldReturnSuccess()
    {
        var personId = Fixture.Create<Guid>();

        _repositoryMock
            .Setup(x => x.DeleteAsync(personId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.DeletePersonAsync(personId);

        AssertSuccess(result);
        result.Value.Should().BeTrue();

        _repositoryMock.Verify(x => x.DeleteAsync(personId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletePersonAsync_WhenPersonNotFound_ShouldReturnNotFound()
    {
        var personId = Fixture.Create<Guid>();

        _repositoryMock
            .Setup(x => x.DeleteAsync(personId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.DeletePersonAsync(personId);

        AssertFailure(result, 404);
        result.ErrorMessage.Should().Be("Not Found.");

        _repositoryMock.Verify(x => x.DeleteAsync(personId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddContactInfoAsync_WithValidRequest_ShouldReturnSuccess()
    {
        var personId = Fixture.Create<Guid>();
        var request = Fixture.Create<AddContactInfoRequest>();

        _addContactInfoValidatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.ExistsAsync(personId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(x => x.AddContactInfoAsync(It.IsAny<PersonContactInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        _repositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.AddContactInfoAsync(personId, request);

        AssertSuccess(result);
        result.Value.Should().NotBe(Guid.Empty);

        _repositoryMock.Verify(x => x.ExistsAsync(personId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.AddContactInfoAsync(It.IsAny<PersonContactInfo>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AddContactInfoAsync_WithInvalidRequest_ShouldReturnValidationFailure()
    {
        var personId = Fixture.Create<Guid>();
        var request = Fixture.Create<AddContactInfoRequest>();
        var validationFailures = new List<ValidationFailure>
        {
            new("Content", "Content cannot be empty"),
            new("Type", "Invalid contact type")
        };
        var validationResult = new ValidationResult(validationFailures);

        _addContactInfoValidatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var result = await _service.AddContactInfoAsync(personId, request);

        AssertFailure(result, 400);
        result.ErrorMessage.Should().Contain("Content cannot be empty");
        result.ErrorMessage.Should().Contain("Invalid contact type");

        _repositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(x => x.AddContactInfoAsync(It.IsAny<PersonContactInfo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddContactInfoAsync_WhenPersonNotFound_ShouldReturnNotFound()
    {
        var personId = Fixture.Create<Guid>();
        var request = Fixture.Create<AddContactInfoRequest>();

        _addContactInfoValidatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.ExistsAsync(personId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.AddContactInfoAsync(personId, request);

        AssertFailure(result, 404);
        result.ErrorMessage.Should().Be("Not Found.");

        _repositoryMock.Verify(x => x.ExistsAsync(personId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.AddContactInfoAsync(It.IsAny<PersonContactInfo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveContactInfoAsync_WhenContactExists_ShouldReturnSuccess()
    {
        var personId = Fixture.Create<Guid>();
        var contactInfoId = Fixture.Create<Guid>();

        _repositoryMock
            .Setup(x => x.RemoveContactInfoAsync(personId, contactInfoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.RemoveContactInfoAsync(personId, contactInfoId);

        AssertSuccess(result);
        result.Value.Should().BeTrue();

        _repositoryMock.Verify(x => x.RemoveContactInfoAsync(personId, contactInfoId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveContactInfoAsync_WhenContactNotFound_ShouldReturnNotFound()
    {
        var personId = Fixture.Create<Guid>();
        var contactInfoId = Fixture.Create<Guid>();

        _repositoryMock
            .Setup(x => x.RemoveContactInfoAsync(personId, contactInfoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.RemoveContactInfoAsync(personId, contactInfoId);

        AssertFailure(result, 404);
        result.ErrorMessage.Should().Be("Not Found");

        _repositoryMock.Verify(x => x.RemoveContactInfoAsync(personId, contactInfoId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllPersonsAsync_ShouldReturnPersonsList()
    {
        var persons = Fixture.CreateMany<Person>(3).ToList();

        _repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(persons);

        var result = await _service.GetAllPersonsAsync();

        AssertSuccess(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().AllSatisfy(person =>
        {
            person.Should().NotBeNull();
            person.Id.Should().NotBeEmpty();
            person.FirstName.Should().NotBeNullOrEmpty();
            person.LastName.Should().NotBeNullOrEmpty();
            person.Company.Should().NotBeNullOrEmpty();
        });

        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAllPersonsAsync_WhenNoPersons_ShouldReturnEmptyList()
    {
        _repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Person>());

        var result = await _service.GetAllPersonsAsync();

        AssertSuccess(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPersonWithContactInfosAsync_WhenPersonExists_ShouldReturnPersonWithContactInfos()
    {
        var person = Fixture.Create<Person>();
        person.ContactInfos = Fixture.CreateMany<PersonContactInfo>(2).ToList();

        _repositoryMock
            .Setup(x => x.GetPersonWithContactInfosAsync(person.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        var result = await _service.GetPersonWithContactInfosAsync(person.Id);

        AssertSuccess(result);
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(person.Id);
        result.Value.FirstName.Should().Be(person.FirstName);
        result.Value.LastName.Should().Be(person.LastName);
        result.Value.Company.Should().Be(person.Company);
        result.Value.ContactInfos.Should().HaveCount(2);

        _repositoryMock.Verify(x => x.GetPersonWithContactInfosAsync(person.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPersonWithContactInfosAsync_WhenPersonNotFound_ShouldReturnNotFound()
    {
        var personId = Fixture.Create<Guid>();

        _repositoryMock
            .Setup(x => x.GetPersonWithContactInfosAsync(personId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        var result = await _service.GetPersonWithContactInfosAsync(personId);

        AssertFailure(result, 404);
        result.ErrorMessage.Should().Be("Not Found.");

        _repositoryMock.Verify(x => x.GetPersonWithContactInfosAsync(personId, It.IsAny<CancellationToken>()), Times.Once);
    }
}