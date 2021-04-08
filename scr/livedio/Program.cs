using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace livedio
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await Inicio();
        }

        public static async Task Inicio()
        {
            Console.WriteLine("Live Dio - Service Bus");
            Console.WriteLine("Digite 1: Para produzir mensagens na fila");
            Console.WriteLine("Digite 2: Para produzir mensagens no tópico");
            Console.WriteLine("Digite 2: Para Leitura de mensagens");
            var resposta = Console.ReadLine();

            if (resposta == "1")
            {
                await new ProdutorFila().ProduzirMensagem();
                await Inicio();
            }
            else if (resposta == "2")
            {
                await new ProdutorTopico().ProduzirMensagem();
                await Inicio();
            }
            else if (resposta == "3")
                CreateHostBuilder().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder() =>
             Host.CreateDefaultBuilder()
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddHostedService<ConsumidorFilaMorta>();
                     services.AddHostedService<ConsumidorTopico>();
                 });
    }
}