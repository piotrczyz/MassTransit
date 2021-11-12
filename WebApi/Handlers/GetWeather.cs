using System.Threading.Tasks;
using MassTransit;

namespace WebApi.Handlers
{
    public class GetWeatherCommandHandler : IConsumer<GetWeatherCommand>
    {
        public Task Consume(ConsumeContext<GetWeatherCommand> context)
        {
            var command = context.Message;


            return Task.CompletedTask;
        }
    }

    public class GetWeatherCommand
    {
    }
}