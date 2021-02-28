using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using PerformanceCommand;

using Microsoft.Extensions.Logging;
using MMDHelpers.CSharp.PerformanceChecks;

namespace MMDHelpers.CSharp.Performance
{
    public class LogService : PerformanceCommand.Sender.SenderBase
    {
        private readonly ILogger _logger;

        public LogService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LogService>();
        }

        public async override Task PerformanceCentral(
            IAsyncStreamReader<CommandRequest> requestStream,
            IServerStreamWriter<CommandResult> responseStream,
            ServerCallContext context)
        {

            while (await requestStream.MoveNext())
            {
                var command = requestStream.Current.Command;
                var response = "";
                if (command > 0)
                {
                    if (command == 0)
                    {
                        response = "0[0-1] = help\n1 = StartMeasure(ignorePastGCCollections)\n2 = StopMeasurement\n3[0-1] = Collect Data(ignorePastGCCollections)\n4 = LogtoFile";
                    }
                    if (command == 1)
                    {
                        Ruler.StartMeasuring(requestStream.Current.State);
                    }
                    if (command == 2)
                    {
                        Ruler.StopMeasuring();
                    }
                    if (command == 3)
                    {
                        response = Ruler.Show(requestStream.Current.State);
                    }
                    if (command == 4)
                    {
                        Ruler.LogToFile();
                    }
                }
                await responseStream.WriteAsync(new CommandResult
                {
                    Reason = response
                });
            }
        }
    }
}
