using System;
using System.Collections.Generic;

namespace Zoka.ZScript
{
	class VariableExpression : IZScriptExpression
	{
		readonly string										m_VariableName;

		public VariableExpression(string _variable_name)
		{
			m_VariableName = _variable_name;
		}

		/// <inheritdoc />
		public object EvaluateExpressionToValue(DataStorages _data_storages, IServiceProvider _service_provider)
		{
			foreach (var data_storage in _data_storages)
			{
				if (data_storage.ContainsKey(m_VariableName))
					return data_storage[m_VariableName];
			}

			throw new KeyNotFoundException($"Variable '{m_VariableName}' not found in the storage");
		}

		/// <inheritdoc />
		public string OriginalExpression => m_VariableName;

		/// <inheritdoc />
		public override string ToString()
		{
			return OriginalExpression;
		}

	}
}
