using System.Linq.Expressions;
using AutoFixture;
using ContactReports.Application.Dtos;
using ContactReports.Application.Interfaces;
using ContactReports.Application.Services;
using ContactReports.Domain.Entities;
using ContactReports.Domain.Enums;
using FluentAssertions;
using Moq;

namespace ContactReports.Tests;

public class ReportServiceTests : TestBase
{
    private readonly Mock<IReportRepository> _repositoryMock;
    private readonly Mock<IReportQueueService> _queueServiceMock;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _repositoryMock = Fixture.Freeze<Mock<IReportRepository>>();
        _queueServiceMock = Fixture.Freeze<Mock<IReportQueueService>>();
        _service = new ReportService(_repositoryMock.Object, _queueServiceMock.Object);
    }

    [Fact]
    public async Task GetReports_ShouldReturnSuccess()
    {
        var reports = Fixture.CreateMany<Report>(2).ToList();
        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reports);

        var result = await _service.GetReports(CancellationToken.None);

        AssertSuccess(result);
        result.Value.Should().HaveCount(2);

        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetReportDetails_ShouldReturnSuccess()
    {
        var details = Fixture.CreateMany<ReportDetail>(2).ToList();
        _repositoryMock.Setup(x => x.GetReportDetailsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(details);

        var result = await _service.GetReportDetails(Guid.NewGuid(), CancellationToken.None);

        AssertSuccess(result);
        result.Value.Should().HaveCount(2);

        _repositoryMock.Verify(x => x.GetReportDetailsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateReport_ShouldAddAndQueue()
    {
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Report { Id = Guid.NewGuid() });

        await _service.CreateReport(CancellationToken.None);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _queueServiceMock.Verify(x => x.Send(It.IsAny<ReportQueueRequest>()), Times.Once);
    }

    [Fact]
    public async Task AddReportDetails_WithPreparingReport_ShouldUpdateAndSave()
    {
        var id = Guid.NewGuid();
        var detailsReq = Fixture.CreateMany<ReportDetailsRequest>(2).ToList();
        var preparingReport = new Report { Id = id, Status = ReportStatus.Preparing };

        _repositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Report, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preparingReport);

        await _service.AddReportDetails(id, detailsReq, CancellationToken.None);

        preparingReport.Status.Should().Be(ReportStatus.Completed);
        preparingReport.ReportDetails.Should().HaveCount(2);
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddReportDetails_WithMissingReport_ShouldReturnFailure()
    {
        // Arrange
        _repositoryMock.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<Report, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report)null!);

        // Act
        var result = await _service.AddReportDetails(Guid.NewGuid(), new List<ReportDetailsRequest>(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Report not found");
    }
}