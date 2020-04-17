using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Client.Abstractions;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Client.ConsoleClient.Utils;
using Core.Client.Shared;
using Core.Client.Utils;
using Newtonsoft.Json;

namespace Core.Client.ConsoleClient {
	public sealed class ConsoleRunner<TConfig, TState> where TConfig : IConfig where TState : IState {
		readonly CommandProvider<TConfig, TState> _commandProvider;
		readonly ConsoleReader                    _reader;
		readonly IClient<TConfig, TState>         _client;

		readonly JsonObjectPresenter _presenter = new JsonObjectPresenter(
			new JsonSerializerSettings { Formatting = Formatting.Indented });

		public ConsoleRunner(
			CommandProvider<TConfig, TState> commandProvider, ConsoleReader reader,
			IClient<TConfig, TState> client) {
			_commandProvider = commandProvider;
			_reader          = reader;
			_client          = client;
		}

		public async Task Run() {
			_client.StateUpdated += () => {
				Console.WriteLine("State updated");
				DrawState();
				DrawCommands();
			};
			var initializeResult = await _client.Initialize(CancellationToken.None);
			if ( initializeResult is InitializationResult.Error e ) {
				Console.WriteLine($"Initialization failed with '{e.Description}'");
				return;
			}
			while ( true ) {
				var finished = await UpdateLoop();
				if ( finished ) {
					return;
				}
			}
		}

		async Task<bool> UpdateLoop() {
			DrawState();
			DrawCommands();
			var selection = ReadSelection();
			if ( selection == 0 ) {
				return true;
			}
			if ( selection > 0 ) {
				await ExecuteCommand(selection - 1);
			}
			Console.WriteLine();
			return false;
		}

		async Task ExecuteCommand(int index) {
			var command = CreateCommand(index);
			var result  = await _client.Apply(command, CancellationToken.None);
			if ( result is CommandApplyResult.Error error ) {
				Console.WriteLine($"Error: '{error.Description}'");
			}
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

		int ReadSelection() => _reader.Read<int>();

		ICommand<TConfig, TState> CreateCommand(int index) {
			var commands = _commandProvider.CommandTypes;
			var type     = commands[index];
			Console.WriteLine($"Execute command: {type.Name}");
			var instance = Activator.CreateInstance(type);
			TryAddPropertyValues(instance);
			var commandInstance = (ICommand<TConfig, TState>) instance;
			return commandInstance;
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