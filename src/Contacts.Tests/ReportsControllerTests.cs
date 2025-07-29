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
    public class ReportsControllerTests : TestBase
    {
        private readonly Mock<IReportService> _mockService;
        private readonly ReportsController _controller;

        public ReportsControllerTests()
        {
            _mockService = Fixture.Freeze<Mock<IReportService>>();
            _controller = new ReportsController(_mockService.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenReportsExist()
        {
            var reports = Fixture.CreateMany<ReportDto>(3).ToList();
            _mockService.Setup(x => x.GetReport(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<List<ReportDto>>.Success(reports));

            var result = await _controller.Get(CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(reports);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenNoReportsExist()
        {
            var emptyReports = new List<ReportDto>();
            _mockService.Setup(x => x.GetReport(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<List<ReportDto>>.Success(emptyReports));

            var result = await _controller.Get(CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;

            var returnedReports = ok.Value as IEnumerable<ReportDto>;
            returnedReports.Should().NotBeNull();
            returnedReports.Should().BeEmpty();
        }
    }
}