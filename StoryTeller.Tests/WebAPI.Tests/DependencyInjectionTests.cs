using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StoryTeller.API.Middlewares;
using StoryTeller.API.Utils;
using StoryTeller.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests
{
    public class DependencyInjectionTests
    {
        private readonly ServiceProvider _serviceProvider;
        public DependencyInjectionTests()
        {
            var service = new ServiceCollection();
            service.AddWebAPIService(new Mock<IConfiguration>().Object);
            service.AddDbContext<StoryTellerContext>(
                option => option.UseInMemoryDatabase("test"));
            _serviceProvider = service.BuildServiceProvider();
        }

        //[Fact]
        //public void DependencyInjectionTests_ServiceShouldResolveCorrectly()
        //{
        //    var currentTimeServiceResolved = _serviceProvider.GetRequiredService<ICurrentTime>();
        //    var claimsServiceServiceResolved = _serviceProvider.GetRequiredService<IClaimsService>();
        //    var exceptionMiddlewareResolved = _serviceProvider.GetRequiredService<GlobalExceptionMiddleware>();
        //    var performanceMiddleware = _serviceProvider.GetRequiredService<PerformanceMiddleware>();
        //    var stopwatchResolved = _serviceProvider.GetRequiredService<Stopwatch>();
        //    var chemicalServiceResolved = _serviceProvider.GetRequiredService<IChemicalService>();
        //    var chemicalRepositoryResolved = _serviceProvider.GetRequiredService<IChemicalRepository>();

        //    currentTimeServiceResolved.GetType().Should().Be(typeof(CurrentTime));
        //    claimsServiceServiceResolved.GetType().Should().Be(typeof(ClaimsService));
        //    exceptionMiddlewareResolved.GetType().Should().Be(typeof(GlobalExceptionMiddleware));
        //    performanceMiddleware.GetType().Should().Be(typeof(PerformanceMiddleware));
        //    stopwatchResolved.GetType().Should().Be(typeof(Stopwatch));
        //    chemicalServiceResolved.GetType().Should().Be(typeof(ChemicalService));
        //    chemicalRepositoryResolved.GetType().Should().Be(typeof(ChemicalRepository));
        //}
    }
}
