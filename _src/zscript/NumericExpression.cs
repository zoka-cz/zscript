using System;

namespace Zoka.ZScript
{
	class NumericExpression : IZScriptExpression
	{
		readonly int										m_Number;
		
		public NumericExpression(int _number)
		{
			m_Number = _number;
		}

		/// <inheritdoc />
		public object EvaluateExpressionToValue(DataStorages _data_storages, IServiceProvider _service_provider)
		{
			return m_Number;
		}

		/// <inheritdoc />
		public string OriginalExpression => m_Number.ToString();

		/// <inheritdoc />
		public override string ToString()
		{
			return OriginalExpression;
		}

	}
}
