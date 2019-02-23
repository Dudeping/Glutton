using Codeping.Glutton.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Codeping.Glutton.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddGluttonCore();

            services.AddLogging();

            var provider = services.BuildServiceProvider();

            var requestor = provider.GetService<IRequestor>();

            //var context = new RequestContext("https://www.layui.com/", "test", false)
            var context = new RequestContext("https://www.layui.com/admin/std/dist/views/", "test1", false)
            {
                IsOnlySubdirectory = true,
                OnChange = OnChange
            };

            Task.Run(async () =>
            {
                try
                {
                    await requestor.RequestAsync(context);

                    Console.WriteLine("OK!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });

            Console.ReadKey();
        }

        static void OnChange(NotifyInfo info)
        {
            if (info.State == MessageState.Failed)
            {
                var content = $"{info.State} : {info.Node.OriginalString} \r\n { (info.HasException ? info.RawException.ToString() : "") }";

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(content);

                Console.ResetColor();

                File.AppendAllText("error.log", content);
            }
            else
            {
                Console.WriteLine($"{info.State} : {info.Node.OriginalString}");
            }
        }
    }
}
