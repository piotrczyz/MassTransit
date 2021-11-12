using System.Net;
using System.Threading.Tasks;
using MassTransit.Testing;
using WebApi;
using WebApi.Handlers;
using Xunit;
using Assert = Xunit.Assert;

namespace IntegrationTests
{
    public class GetWeatherForecastTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        
        public GetWeatherForecastTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
        
        [Fact]                                                 
        public async Task Call_Api()
        {   
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync($"WeatherForecast/Single");
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Harness()
        {
            //arrange
            var harness = new InMemoryTestHarness();
            var handlerHarness = harness.Consumer(() =>
                new GetWeatherCommandHandler());
            
            await harness.Start();

            var command = new GetWeatherCommand();
            
            try
            {
                //act
                await harness.InputQueueSendEndpoint.Send<GetWeatherCommand>(command);
                
                //assert
                Assert.True(await harness.Consumed.Any<GetWeatherCommand>());

                Assert.True(await handlerHarness.Consumed.Any<GetWeatherCommand>());

            }
            finally
            {
                await harness.Stop();
            }   
        }
    }
}