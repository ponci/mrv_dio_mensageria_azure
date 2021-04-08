using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace livedio
{
    public class ConsumidorFila : BackgroundService
    {
        private readonly QueueClient _client;

        public ConsumidorFila()
        {
            _client = new QueueClient(
                "ConnectionString",
                "fila1",
                ReceiveMode.PeekLock);
            Console.WriteLine("Iniciando a leitura da fila no ServiceBus...");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    _client.RegisterMessageHandler(ProcessarMensagem,
                        new MessageHandlerOptions(ProcessarErro)
                        {
                            MaxConcurrentCalls = 1,
                            AutoComplete = false
                        }
                    );
                });
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _client.CloseAsync();
            Console.WriteLine("Finalizando conexão com o Azure Service");
        }

        private async Task ProcessarMensagem(Message message, CancellationToken token)
        {
            var corpo = Encoding.UTF8.GetString(message.Body);

            Console.WriteLine("[Nova Mensagem Recebida na fila] " + corpo);

            await _client.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ProcessarErro(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("[Erro] " +
                e.Exception.GetType().FullName + " " +
                e.Exception.Message);
            return Task.CompletedTask;
        }
    }
}