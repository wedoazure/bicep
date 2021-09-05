// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.JSInterop;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Linq;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Bicep.Core.Extensions;
using Bicep.Decompiler;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using System.IO.Pipelines;
using Bicep.LanguageServer;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Concurrency;
using Bicep.Core.Resources;
using Bicep.LanguageServer.Snippets;
using BlazorWorker.WorkerCore;

namespace Bicep.Wasm
{
    public class Worker
    {
        private class TestResourceTypeLoader : IResourceTypeLoader
        {
            public IEnumerable<ResourceTypeReference> GetAvailableTypes()
                => Enumerable.Empty<ResourceTypeReference>();

            public ResourceType LoadType(ResourceTypeReference reference)
            {
                throw new NotImplementedException();
            }
        }

        private class TestSnippetsProvider : ISnippetsProvider
        {
            public IEnumerable<Snippet> GetModuleBodyCompletionSnippets(TypeSymbol typeSymbol)
                => Enumerable.Empty<Snippet>();

            public IEnumerable<Snippet> GetNestedResourceDeclarationSnippets(ResourceTypeReference resourceTypeReference)
                => Enumerable.Empty<Snippet>();

            public IEnumerable<Snippet> GetObjectBodyCompletionSnippets(TypeSymbol typeSymbol)
                => Enumerable.Empty<Snippet>();

            public IEnumerable<Snippet> GetResourceBodyCompletionSnippets(ResourceType resourceType, bool isExistingResource, bool isResourceNested)
                => Enumerable.Empty<Snippet>();

            public IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets()
                => Enumerable.Empty<Snippet>();
        }

        private readonly Server server;
        private readonly PipeWriter inputWriter;
        private readonly PipeReader outputReader;
        private readonly IWorkerMessageService messageService;

        public Worker(IWorkerMessageService messageService)
        {
            this.messageService = messageService;
            this.messageService.IncomingMessage += OnMessage;

            var inputPipe = new Pipe();
            var outputPipe = new Pipe();

            server = new Server(inputPipe.Reader, outputPipe.Writer, new Server.CreationOptions {
                SnippetsProvider = new TestSnippetsProvider(),
                FileResolver = new FileResolver(),
                ResourceTypeProvider = AzResourceTypeProvider.CreateWithLoader(new TestResourceTypeLoader(), false),
            }, options =>  options.Services.AddSingleton<IScheduler>(ImmediateScheduler.Instance));

            inputWriter = inputPipe.Writer;
            outputReader = outputPipe.Reader;

#pragma warning disable VSTHRD110
            Task.Run(() => server.RunAsync(CancellationToken.None));
            Task.Run(() => ProcessInputStreamAsync());
#pragma warning restore VSTHRD110
        }

        public void OnMessage(object? sender, string message)
        {
            if (message.StartsWith("SND:"))
            {
                inputWriter.WriteAsync(Encoding.UTF8.GetBytes(message.Substring(4))).ConfigureAwait(false);
            }
        }

        private async Task ProcessInputStreamAsync()
        {
            try
            {
                do
                {
                    var result = await outputReader.ReadAsync(CancellationToken.None).ConfigureAwait(false);
                    var buffer = result.Buffer;

                    var message = Encoding.UTF8.GetString(buffer.Slice(buffer.Start, buffer.End));
                    await messageService.PostMessageAsync($"RCV:{message}");
                    outputReader.AdvanceTo(buffer.End, buffer.End);

                    // Stop reading if there's no more data coming.
                    if (result.IsCompleted && buffer.IsEmpty)
                    {
                        break;
                    }
                    // TODO: Add cancellation token
                } while (!CancellationToken.None.IsCancellationRequested);
            }
            catch (Exception e)
            {
                // TODO: Needed?
                await Console.Error.WriteLineAsync(e.Message);
                await Console.Error.WriteLineAsync(e.StackTrace);
            }
        }
    }
}