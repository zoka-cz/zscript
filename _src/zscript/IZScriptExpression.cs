using System;

namespace Zoka.ZScript
{
	/// <summary>Interface for single expression of the ZScript</summary>
	public interface IZScriptExpression
	{
		/// <summary>Will evaluate the expression into the value</summary>
		object EvaluateExpressionToValue(DataStorages _data_storages, IServiceProvider _service_provider);

		/// <summary>The expression as originally written</summary>
		string OriginalExpression { get; }
	}
}
