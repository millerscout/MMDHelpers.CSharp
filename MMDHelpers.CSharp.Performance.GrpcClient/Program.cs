using Grpc.Core;
using Grpc.Net.Client;
using PerformanceCommand;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace client
{
    static class Program
    {
        static async Task Main(string[] args)
        {


            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(args.Length == 0 ? "https://localhost:5001" : args[0],
                new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new PerformanceCommand.Sender.SenderClient(channel);

            Console.WriteLine("Client created");

            using (var call = client.PerformanceCentral(headers: new Metadata { new Metadata.Entry("debug", "1") }))
            {
                var responseTask = Task.Run(async () =>
                {
                    await foreach (var message in call.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine(message.Reason);
                    }
                });

                await call.RequestStream.WriteAsync(new CommandRequest() { Command = 1, State = false });
                while (true)
                {
                    var result = Console.ReadKey(intercept: true);
                    if (result.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    var command = Console.ReadLine();
                    if (command.Length > 0)
                    {
                        var param = Convert.ToBoolean(command.Length == 2 ? command[1] - '0' : 0);
                        if (command[0] - '0' == 0)
                        {
                            Console.WriteLine("0[0-1] = help\n1 = StartMeasure(ignorePastGCCollections)\n2 = StopMeasurement\n3[0-1] = Collect Data(ignorePastGCCollections)\n4 = LogtoFile");
                        }
                        if (command[0] - '0' == 1)
                        {
                            await call.RequestStream.WriteAsync(new CommandRequest() { Command = 1, State = param });
                        }
                        if (command[0] - '0' == 2)
                        {
                            await call.RequestStream.WriteAsync(new CommandRequest() { Command = 2, State = param });
                        }
                        if (command[0] - '0' == 3)
                        {
                            await call.RequestStream.WriteAsync(new CommandRequest() { Command = 3, State = param });
                        }
                        if (command[0] - '0' == 3)
                        {
                            await call.RequestStream.WriteAsync(new CommandRequest() { Command = 4, State = param });
                        }
                    }

                }

                Console.WriteLine("Disconnecting");
                await call.RequestStream.CompleteAsync();
                await responseTask;
            }

            Console.WriteLine("Disconnected. Press any key to exit.");
            Console.ReadKey();
        }

    }
}
