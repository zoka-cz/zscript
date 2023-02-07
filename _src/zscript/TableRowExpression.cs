using System;
using System.Collections.Generic;
using System.Data;

namespace Zoka.ZScript
{
	internal class TableRowExpression : IZScriptExpression
	{
		readonly IZScriptExpression							m_Row;
		readonly IZScriptExpression							m_ColumnIndexer;

		public TableRowExpression(string _table_name, IZScriptExpression _column_indexer)
		{
			m_Row = new VariableExpression(_table_name);
			m_ColumnIndexer = _column_indexer;
		}
		/// <inheritdoc />
		public object										EvaluateExpressionToValue(DataStorages _data_storages, IServiceProvider _service_provider)
		{
			var dt = m_Row.EvaluateExpressionToValue(_data_storages, _service_provider) as DataRow;

			if (dt == null)
				throw new KeyNotFoundException($"Table row {m_Row.OriginalExpression} not found");

			var column_index = m_ColumnIndexer.EvaluateExpressionToValue(_data_storages, _service_provider);

			if (column_index is string)
				return dt[(string)column_index];
			else if (column_index is int)
			{
				return dt[(int)column_index];
			}

			throw new FormatException("Column index must resolve to string or to int.");
		}

		/// <inheritdoc />
		public string										OriginalExpression => $"{m_Row.OriginalExpression}[{m_ColumnIndexer.OriginalExpression}]";

		/// <inheritdoc />
		public override string								ToString()
		{
			return OriginalExpression;
		}

	}
}
