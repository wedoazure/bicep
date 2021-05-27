// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { registerCommand } from "vscode-azureextensionui";

import { Disposable } from "../utils/disposable";
import { Command } from "./types";

export class CommandManager extends Disposable {
  public registerCommand<T extends Command>(command: T): void {
    registerCommand(command.id, command.execute.bind(command))
  }

  public registerCommands<T extends [Command, ...Command[]]>(
    ...commands: T
  ): void {
    commands.map((command) => this.registerCommand(command));
  }
}
