// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Reactive.PlatformServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Core;

namespace Bicep.Wasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddWorkerFactory();
            builder.Services.AddSingleton<WorkerHost>();

            var workerHost = builder.Services.BuildServiceProvider().GetService<WorkerHost>() ?? throw new InvalidOperationException($"Failed to load {nameof(WorkerHost)}.");
            await workerHost.InitializeAsync();

            await builder.Build().RunAsync();
        }
    }
}
