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
    public class ConsumidorTopico : BackgroundService
    {
        private readonly SubscriptionClient _client;
        private readonly ILogger<ConsumidorTopico> _log;

        public ConsumidorTopico(ILogger<ConsumidorTopico> log)
        {
            _log = log;

            string connectionString = "ConnectionString";
            string topico = "topico1";
            string assinatura = "assinatura1";

            _client = new SubscriptionClient(connectionString,
                topico, assinatura);
            Console.WriteLine("Iniciando a leitura do topico no ServiceBus...");
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
                            MaxConcurrentCalls = 5,
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

            Console.WriteLine("[Nova Mensagem Recebida no topico] " + corpo);

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