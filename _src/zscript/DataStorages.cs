using System.Collections.Generic;

namespace Zoka.ZScript
{
	/// <summary>Data storage class</summary>
	public class DataStorage : Dictionary<string, object>
	{
	}

	/// <summary>Data storages</summary>
	public class DataStorages : Stack<DataStorage>
	{
		/// <summary>Will return the object from data storage</summary>
		public object										GetObjectFromDataStorage(string _variable_name, bool _throw_not_found = true)
		{
			foreach (var data_storage in this)
			{
				if (data_storage.ContainsKey(_variable_name))
					return data_storage[_variable_name];
			}

			if (_throw_not_found)
				throw new KeyNotFoundException($"Variable {_variable_name} not found.");

			return null;
		}

		/// <summary>Will store the data into storage</summary>
		public void											Store(string _variable_name, object _value)
		{
			foreach (var data_storage in this)
			{
				if (data_storage.ContainsKey(_variable_name))
				{
					data_storage[_variable_name] = _value;
					return;
				}
			}

			var current_storage = Peek();
			current_storage[_variable_name] = _value;
		}
	}
}
