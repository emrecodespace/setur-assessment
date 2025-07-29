using ContactReports.Api.Controllers;
using ContactReports.Application.Common;
using ContactReports.Application.Dtos;
using ContactReports.Application.Interfaces;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactReports.Tests
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
        public async Task GetReports_ShouldReturnOk_WhenReportsExist()
        {
            var reports = Fixture.CreateMany<ReportDto>(3).ToList();
            _mockService.Setup(x => x.GetReports(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<ReportDto>>.Success(reports));

            var result = await _controller.GetReports(CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(reports);
        }

        [Fact]
        public async Task GetReports_ShouldReturnOk_WhenNoReportsExist()
        {
            _mockService.Setup(x => x.GetReports(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<ReportDto>>.Success(new List<ReportDto>()));

            var result = await _controller.GetReports(CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().NotBeNull();
            ((IEnumerable<ReportDto>)ok.Value!).Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetDetails_ShouldReturnOk_WhenExists()
        {
            var id = Fixture.Create<Guid>();
            var detailsList = Fixture.CreateMany<ReportDetailsDto>(3).ToList();

            _mockService.Setup(x => x.GetReportDetails(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<ReportDetailsDto>>.Success(detailsList));

            var result = await _controller.GetDetails(id, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(detailsList);
        }

        [Fact]
        public async Task GetDetails_ShouldReturnNotFound_WhenNotExists()
        {
            var id = Fixture.Create<Guid>();

            _mockService.Setup(x => x.GetReportDetails(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<ReportDetailsDto>>.Failure("Not found", 404));

            var result = await _controller.GetDetails(id, CancellationToken.None);

            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value?.ToString().Should().Contain("Not found");
        }
        
        [Fact]
        public async Task CreateReport_ShouldReturnOk_WhenSuccessful()
        {
            _mockService.Setup(x => x.CreateReport(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _controller.CreateReport(CancellationToken.None);

            result.Should().BeOfType<OkResult>();
        }
        
        [Fact]
        public async Task CreateReport_ShouldReturnBadRequest_WhenFailed()
        {
            _mockService.Setup(x => x.CreateReport(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Guid>.Failure("Cannot create report", 400));

            var result = await _controller.CreateReport(CancellationToken.None);

            var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            bad.Value?.ToString().Should().Contain("Cannot create report");
        }

        [Fact]
        public async Task CreateReportDetails_ShouldReturnOk_WhenSuccessful()
        {
            var reportId = Fixture.Create<Guid>();
            var details = Fixture.CreateMany<ReportDetailsRequest>(2).ToList();
            _mockService.Setup(x => x.AddReportDetails(reportId, details, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await _controller.CreateReport(reportId, details, CancellationToken.None);

            result.Should().BeOfType<OkResult>();
        }
        
        [Fact]
        public async Task CreateReportDetails_ShouldReturnNotFound_WhenReportNotFound()
        {
            var reportId = Fixture.Create<Guid>();
            var details = Fixture.CreateMany<ReportDetailsRequest>(2).ToList();
            _mockService.Setup(x => x.AddReportDetails(reportId, details, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Failure("Report not found", 404));

            var result = await _controller.CreateReport(reportId, details, CancellationToken.None);

            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value?.ToString().Should().Contain("Report not found");
        }
    }
}