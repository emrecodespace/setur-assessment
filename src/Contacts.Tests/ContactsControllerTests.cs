using AutoFixture;
using Contacts.Api.Controllers;
using Contacts.Application.Common;
using Contacts.Application.Dtos;
using Contacts.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Contacts.Tests
{
    public class ContactsControllerTests : TestBase
    {
        private readonly Mock<IContactService> _mockService;
        private readonly ContactsController _controller;

        public ContactsControllerTests()
        {
            _mockService = Fixture.Freeze<Mock<IContactService>>();
            _controller = new ContactsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllPersons_ShouldReturnOk_WhenPersonsExist()
        {
            var persons = Fixture.CreateMany<PersonsDto>(3).ToList();
            _mockService.Setup(x => x.GetAllPersonsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<PersonsDto>>.Success(persons));

            var result = await _controller.GetAllPersons(CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(persons);
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnOk_WhenValid()
        {
            var req = Fixture.Build<CreatePersonRequest>().With(x => x.FirstName, "Test User").With(x => x.LastName, "Test Last").Create();
            var id = Fixture.Create<Guid>();
            _mockService.Setup(x => x.CreatePersonAsync(req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Guid>.Success(id));

            var result = await _controller.CreatePerson(req, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(id);
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnBadRequest_WhenFirstNameOrLastNameMissing()
        {
            var req = Fixture.Build<CreatePersonRequest>()
                .With(x => x.FirstName, "")
                .With(x => x.LastName, "")
                .Create();

            var errors = new[] { "FirstName is required", "LastName is required" };
            var errorString = string.Join("; ", errors);

            _mockService.Setup(x => x.CreatePersonAsync(req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Guid>.Failure(errorString, 400));

            var result = await _controller.CreatePerson(req, CancellationToken.None);

            var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            bad.Value?.ToString().Should().Contain("FirstName is required");
            bad.Value?.ToString().Should().Contain("LastName is required");
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnOk_WhenExists()
        {
            var id = Fixture.Create<Guid>();
            _mockService.Setup(x => x.DeletePersonAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await _controller.DeletePerson(id, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(true);
        }
        
        [Fact]
        public async Task DeletePerson_ShouldReturnNotFound_WhenNotExists()
        {
            var id = Fixture.Create<Guid>();
            _mockService.Setup(x => x.DeletePersonAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Failure("Not found", 404));

            var result = await _controller.DeletePerson(id, CancellationToken.None);

            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value?.ToString().Should().Contain("Not found");
        }

        [Fact]
        public async Task GetPersonWithContactInfos_ShouldReturnOk()
        {
            var personId = Fixture.Create<Guid>();
            var person = Fixture.Build<PersonDetailsDto>().With(x => x.Id, personId).Create();

            _mockService.Setup(x => x.GetPersonWithContactInfosAsync(personId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PersonDetailsDto?>.Success(person));

            var result = await _controller.GetPersonWithContactInfosAsync(personId, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(person);
        }

        [Fact]
        public async Task GetPersonWithContactInfos_ShouldReturnNotFound_WhenPersonNotExists()
        {
            var personId = Fixture.Create<Guid>();

            _mockService.Setup(x => x.GetPersonWithContactInfosAsync(personId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PersonDetailsDto?>.Failure("Not found", 404));

            var result = await _controller.GetPersonWithContactInfosAsync(personId, CancellationToken.None);

            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value?.ToString().Should().Contain("Not found");
        }

        [Fact]
        public async Task AddContactInfo_ShouldReturnOk_WhenValid()
        {
            var personId = Fixture.Create<Guid>();
            var req = Fixture.Create<AddContactInfoRequest>();
            var contactInfoId = Fixture.Create<Guid>();
            _mockService.Setup(x => x.AddContactInfoAsync(personId, req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Guid>.Success(contactInfoId));

            var result = await _controller.AddContactInfo(personId, req, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(contactInfoId);
        }

        [Fact]
        public async Task AddContactInfo_ShouldReturnBadRequest_WhenInvalid()
        {
            var personId = Fixture.Create<Guid>();
            var req = Fixture.Create<AddContactInfoRequest>();

            var errors = new[] { "Content is required", "Type is invalid" };
            var errorString = string.Join("; ", errors);

            _mockService.Setup(x => x.AddContactInfoAsync(personId, req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Guid>.Failure(errorString, 400)); // <-- STATUS CODE 400 EKLENDİ

            var result = await _controller.AddContactInfo(personId, req, CancellationToken.None);

            var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            bad.Value?.ToString().Should().Contain("Content is required");
            bad.Value?.ToString().Should().Contain("Type is invalid");
        }

        [Fact]
        public async Task RemoveContactInfo_ShouldReturnOk_WhenSuccess()
        {
            var personId = Fixture.Create<Guid>();
            var contactInfoId = Fixture.Create<Guid>();
            _mockService.Setup(x => x.RemoveContactInfoAsync(personId, contactInfoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await _controller.RemoveContactInfo(personId, contactInfoId, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(true);
        }

        [Fact]
        public async Task RemoveContactInfo_ShouldReturnNotFound_WhenNotFound()
        {
            var personId = Fixture.Create<Guid>();
            var contactInfoId = Fixture.Create<Guid>();
            _mockService.Setup(x => x.RemoveContactInfoAsync(personId, contactInfoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Failure("Contact info not found", 404));

            var result = await _controller.RemoveContactInfo(personId, contactInfoId, CancellationToken.None);

            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value?.ToString().Should().Contain("Contact info not found");
        }
    }
}