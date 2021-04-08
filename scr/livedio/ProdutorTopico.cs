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
    public class ProdutorTopico
    {
        private readonly TopicClient _client;

        public ProdutorTopico()
        {
            _client = new TopicClient(
                "ConnectionString",
                "topico1");
        }

        public async Task ProduzirMensagem()
        {
            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine(
                        $"Enviando mensagem: " + i);
                    await _client.SendAsync(
                        new Message(Encoding.UTF8.GetBytes("Número " + i)));
                }

                Console.WriteLine("Concluido o envio das mensagens");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.GetType().FullName} | " +
                             $"Mensagem: {ex.Message}");
            }
            finally
            {
                if (_client != null)
                {
                    await _client.CloseAsync();
                    Console.WriteLine(
                        "Finalizando conexão com ServiceBus");
                }
            }
        }
    }
}