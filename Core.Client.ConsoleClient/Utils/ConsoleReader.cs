using System;
using System.Collections.Generic;

namespace Core.Client.ConsoleClient.Utils {
	public sealed class ConsoleReader {
		readonly Dictionary<Type, Func<object>> _readers = new Dictionary<Type, Func<object>> {
			{ typeof(string), ReadString },
			{ typeof(int), ReadInt }
		};

		public T Read<T>() {
			return (T)Read(typeof(T));
		}

		public object Read(Type type) {
			var reader = _readers.GetValueOrDefault(type);
			if ( reader == null ) {
				Console.WriteLine($"Unsupported type: '{type.Name}'");
				return null;
			}
			return reader();
		}

		static object ReadInt() {
			while ( true ) {
				var line = ReadString();
				if ( int.TryParse(line, out var integer) ) {
					return integer;
				}
				Console.WriteLine("Invalid input");
			}
		}

		static string ReadString() => Console.ReadLine();
	}
}