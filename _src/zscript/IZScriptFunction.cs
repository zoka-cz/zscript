using System;
using System.Collections.Generic;

namespace Zoka.ZScript
{
	/// <summary>Interface for any function of the ZScript</summary>
	public interface IZScriptFunction
	{
		/// <summary>Name of the function</summary>
		string Name { get; }

		/// <summary>Will evaluate function into the resulting value</summary>
		object EvaluateFunctionToValue(List<IZScriptExpression> _arguments, DataStorages _data_storages, IServiceProvider _service_provider);
	}
}
