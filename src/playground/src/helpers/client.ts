import { BaseLanguageClient, CloseAction, createConnection, ErrorAction, MonacoLanguageClient, MonacoServices } from 'monaco-languageclient';
import { onLspData, sendLspData } from './lspInterop';
import { Duplex } from 'stream';
import * as monaco from 'monaco-editor';
import { createMessageConnection } from 'monaco-languageclient/node_modules/vscode-jsonrpc';

function marshalToString(data : any, encoding: BufferEncoding | 'buffer') {
  return Buffer.isBuffer(data) ? data.toString(encoding === 'buffer' ? undefined : encoding) : typeof data === 'string' ? data : data.toString();
}

function createStream(): [NodeJS.ReadableStream, NodeJS.WritableStream] {
  const output = new Duplex({
    write: (data, encoding, next) => {
      sendLspData(marshalToString(data, encoding));
      next();
    },
  });

  onLspData(data => {
    try {
      output.push(marshalToString(data, 'utf-8'), 'utf-8');
      output.push(null);
    }
    catch (e) {
      console.error(e);
    }
  });

  return [output, output];
}

export async function createLanguageClient(): Promise<BaseLanguageClient> {
  (self as any)['monaco'] = monaco;
  MonacoServices.install(monaco);

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