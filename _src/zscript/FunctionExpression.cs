using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Zoka.ZScript
{
	class FunctionExpression : IZScriptExpression
	{
		readonly string										m_FunctionName;
		readonly List<IZScriptExpression>					m_Arguments = new List<IZScriptExpression>();

		public FunctionExpression(string _function_name, IEnumerable<IZScriptExpression> _arguments)
		{
			m_FunctionName = _function_name;
			m_Arguments.AddRange(_arguments);
		}

		/// <inheritdoc />
		public object EvaluateExpressionToValue(DataStorages _data_storages, IServiceProvider _service_provider)
		{
			var script_function_factory = _service_provider.GetRequiredService<ZScriptFunctionFactory>();
			var function = script_function_factory.CreateFunction(m_FunctionName);
			var ret_val = function?.EvaluateFunctionToValue(m_Arguments, _data_storages, _service_provider) ?? throw new Exception($"Function {m_FunctionName} not created successfully.");
			return ret_val;
		}

		/// <inheritdoc />
		public string OriginalExpression => $"$${m_FunctionName}({string.Join(", ", m_Arguments.Select(a => a.OriginalExpression))})";

		/// <inheritdoc />
		public override string ToString()
		{
			return OriginalExpression;
		}
	}
}
