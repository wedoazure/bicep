import { BaseLanguageClient, CloseAction, createConnection, DataCallback, Disposable, ErrorAction, Event, Message, MessageReader, MessageWriter, MonacoLanguageClient, MonacoServices, PartialMessageInfo } from 'monaco-languageclient';
import { onLspData, sendLspData } from './lspInterop';
import { Duplex } from 'stream';
import { editor } from 'monaco-editor';
import * as monaco from 'monaco-editor';
import { createMessageConnection } from 'monaco-languageclient/node_modules/vscode-jsonrpc';

class TestReader implements MessageReader {
  private readonly output: Duplex;
  constructor(output: Duplex) {
    this.output = output;
  }
  onError: Event<Error>;
  onClose: Event<void>;
  onPartialMessage: Event<PartialMessageInfo>;
  dispose(): void {
    throw new Error('Method not implemented.');
  }
  listen(callback: DataCallback): Disposable {
    this.output
    throw new Error('Method not implemented.');
  }
}

class TestWriter implements MessageWriter {
  private readonly output: Duplex;
  constructor(output: Duplex) {
    this.output = output;
  }
  onError: Event<[Error, Message, number]>;
  onClose: Event<void>;
  write(msg: Message): void {
    this.output.write(msg.jsonrpc, 'utf-8');
  }
  dispose(): void { }
}

function marshalToString(data : any, encoding: BufferEncoding | 'buffer') {
  return Buffer.isBuffer(data) ? data.toString(encoding === 'buffer' ? undefined : encoding) : typeof data === 'string' ? data : data.toString();
}

function createStream(): [NodeJS.ReadableStream, NodeJS.WritableStream] {
  const output = new Duplex({
    write: (data, encoding, cb) => {
      sendLspData(marshalToString(data, encoding));
      cb();
    }
  });

  onLspData(data => output.push(marshalToString(data, 'utf8')));

  return [output, output];
}

export async function createLanguageClient(editor: editor.IStandaloneCodeEditor): Promise<BaseLanguageClient> {
  (self as any)['monaco'] = monaco;
  MonacoServices.install(editor);

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