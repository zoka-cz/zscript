using System;

namespace Zoka.ZScript
{
	class LiteralExpression : IZScriptExpression
	{
		readonly string										m_LiteralExpression;

		public LiteralExpression(string _literal_expression)
		{
			m_LiteralExpression = _literal_expression;
		}

		/// <inheritdoc />
		public object EvaluateExpressionToValue(DataStorages _data_storages, IServiceProvider _service_provider)
		{
			return m_LiteralExpression;
		}

		/// <inheritdoc />
		public string OriginalExpression => m_LiteralExpression;

		/// <inheritdoc />
		public override string ToString()
		{
			return OriginalExpression;
		}

	}
}
