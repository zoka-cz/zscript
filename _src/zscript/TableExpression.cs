using System;
using System.Collections.Generic;
using System.Data;

namespace Zoka.ZScript
{
	class TableExpression : IZScriptExpression 
	{
		readonly IZScriptExpression							m_Table;
		readonly IZScriptExpression							m_RowIndexer;
		readonly IZScriptExpression							m_ColumnIndexer;

		public TableExpression(string _table_name, IZScriptExpression _row_indexer, IZScriptExpression _column_indexer)
		{
			m_Table = new VariableExpression(_table_name);
			m_RowIndexer = _row_indexer;
			m_ColumnIndexer = _column_indexer;
		}

		/// <inheritdoc />
		public object EvaluateExpressionToValue(DataStorages _data_storages, IServiceProvider _service_provider)
		{
			var dt = m_Table.EvaluateExpressionToValue(_data_storages, _service_provider) as DataTable;

			if (dt == null)
				throw new KeyNotFoundException($"Table {m_Table.OriginalExpression} not found");

			var row_index = (int)m_RowIndexer.EvaluateExpressionToValue(_data_storages, _service_provider);
			var column_index = m_ColumnIndexer.EvaluateExpressionToValue(_data_storages, _service_provider);

			if (column_index is string)
				return dt.Rows[row_index][(string)column_index];
			else if (column_index is int)
			{
				return dt.Rows[row_index][(int)column_index];
			}

			throw new FormatException("Column index must resolve to string or to int.");
		}

		/// <inheritdoc />
		public string OriginalExpression => $"{m_Table.OriginalExpression}[{m_RowIndexer.OriginalExpression}][{m_ColumnIndexer.OriginalExpression}]";

		/// <inheritdoc />
		public override string ToString()
		{
			return OriginalExpression;
		}

	}
}
