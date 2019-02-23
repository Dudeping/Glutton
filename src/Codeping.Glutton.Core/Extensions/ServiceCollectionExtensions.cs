using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGluttonCore(this IServiceCollection services)
        {
            services.AddHttpClient(HttpClientExstensions.CLIENT_NAME, HttpClientExstensions.GeneralInitialize);

            services.AddTransient<IRecorder, Recorder>();

            services.AddTransient<IRequestor, Requestor>();

            services.AddTransient<IRequestFilter, RequestFilter>();
        }
    }
}
