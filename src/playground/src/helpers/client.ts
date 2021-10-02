import { BaseLanguageClient, CloseAction, createConnection, Disposable, ErrorAction, MonacoLanguageClient, MonacoServices } from 'monaco-languageclient';
import { createMessageConnection, DataCallback, Message, MessageReader, MessageWriter } from 'vscode-jsonrpc';
import { onLspData, sendLspData } from './lspInterop';
// @ts-expect-error
import { CommandsRegistry } from 'monaco-editor/esm/vs/platform/commands/common/commands';

function marshalToString(data : any, encoding: BufferEncoding | 'buffer') {
  return Buffer.isBuffer(data) ? data.toString(encoding === 'buffer' ? undefined : encoding) : typeof data === 'string' ? data : data.toString();
}

function createStream() {
  const reader: MessageReader = {
    onError: undefined,
    onClose: undefined,
    onPartialMessage: undefined,
    listen: function (callback: DataCallback): Disposable {
      onLspData(data => callback({ jsonrpc: marshalToString(data, 'utf-8') }));
      return Disposable.create(() => {});
    },
    dispose: function () {},
  };

  const writer: MessageWriter = {
    onError: undefined,
    onClose: undefined,
    write: async function (msg: Message): Promise<void> {
      sendLspData(msg.jsonrpc);
    },
    dispose: function () {},
    end: function () {},
  }

  return [reader, writer] as const;
}

export async function createLanguageClient(): Promise<BaseLanguageClient> {
  MonacoServices.install(CommandsRegistry);

  const [reader, writer] = createStream();
  const messageConnection = createMessageConnection(reader, writer);

  const client = new MonacoLanguageClient({
    name: "Bicep Monaco Client",
    clientOptions: {
      documentSelector: [{ language: 'bicep' }],
      errorHandler: {
        error: () => ErrorAction.Continue,
        closed: () => CloseAction.DoNotRestart
      }
    },
    connectionProvider: {
      get: (errorHandler, closeHandler) => {
        return Promise.resolve(createConnection(messageConnection, errorHandler, closeHandler))
      }
    }
  });

  client.start();
  await client.onReady();

  return client;
}