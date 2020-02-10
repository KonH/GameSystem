using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Core.Client;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.ConsoleClient.Utils;

namespace Core.ConsoleClient {
	public sealed class StandaloneConsoleClient<TConfig, TState>
		where TConfig : IConfig where TState : IState {
		readonly CommandProvider<TConfig, TState> _commandProvider;

		readonly ConsoleReader _reader = new ConsoleReader();

		readonly JsonObjectPresenter _presenter = new JsonObjectPresenter(
			new JsonSerializerOptions { WriteIndented = true });

		readonly StandaloneClient<TConfig, TState> _client;

		public StandaloneConsoleClient(
			CommandProvider<TConfig, TState> commandProvider, CommandQueue<TConfig, TState> queue,
			TConfig config, StateFactory<TState> stateFactory) {
			_commandProvider = commandProvider;
			var loggerFactory = new LoggerFactory(typeof(ConsoleLogger<>));
			_client = new StandaloneClient<TConfig, TState>(loggerFactory, queue, config, stateFactory);
		}

		public void Run() {
			while ( true ) {
				var finished = UpdateLoop();
				if ( finished ) {
					return;
				}
			}
		}

		bool UpdateLoop() {
			DrawState();
			DrawCommands();
			var selection = _reader.Read<int>();
			if ( selection == 0 ) {
				return true;
			}
			if ( selection > 0 ) {
				ExecuteCommand(selection - 1);
			}
			return false;
		}

		void DrawState() {
			Console.WriteLine("State:");
			Console.WriteLine(_presenter.Format(_client.State));
			Console.WriteLine();
		}

		void DrawCommands() {
			Console.WriteLine("Commands:");
			Console.WriteLine("0) Exit");
			var commands = _commandProvider.CommandTypes;
			for ( var i = 0; i < commands.Count; i++ ) {
				var type = commands[i].Name;
				Console.WriteLine($"{(i + 1).ToString()}) {type}");
			}
			Console.WriteLine();
		}

		void ExecuteCommand(int index) {
			var commands = _commandProvider.CommandTypes;
			var type = commands[index];
			Console.WriteLine($"Execute command: {type.Name}");
			var instance = Activator.CreateInstance(type);
			TryAddPropertyValues(instance);
			var commandInstance = (ICommand<TConfig, TState>)instance;
			var result = _client.Apply(commandInstance);
			if ( result is BatchCommandResult<TConfig, TState>.BadCommandResult badResult ) {
				Console.WriteLine($"Command failed with '{badResult.Description}', rewind state");
				_client.Rewind();
			}
			Console.WriteLine();
		}

		void TryAddPropertyValues(object instance) {
			var properties = instance.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanWrite);

			foreach ( var prop in properties ) {
				Console.WriteLine($"Enter property '{prop.Name}' value:");
				var value = _reader.Read(prop.PropertyType);
				if ( value != null ) {
					prop.SetValue(instance, value);
				}
			}
		}
	}
}