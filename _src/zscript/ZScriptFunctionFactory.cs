using System;
using System.Collections.Generic;

namespace Zoka.ZScript
{
	/// <summary>Factory of ZScript functions</summary>
	public class ZScriptFunctionFactory
	{
		private Dictionary<string, Type>					m_ScriptFunctions = new Dictionary<string, Type>();


		/// <summary>Will register the ZScript function worker</summary>
		public void											RegisterScriptFunctionType(string _function_name, Type _function_type)
		{
			if (_function_type == null)
				throw new ArgumentNullException(nameof(_function_type));
			if (!typeof(IZScriptFunction).IsAssignableFrom(_function_type))
				throw new ArgumentException($"Type {_function_type.FullName} is not of IScriptFunction", nameof(_function_type));

			m_ScriptFunctions[_function_name] = _function_type;
		}

		/// <summary>Will instantiate the ZScript function according to its name</summary>
		public IZScriptFunction								CreateFunction(string _function_name)
		{
			if (!m_ScriptFunctions.ContainsKey(_function_name))
				return null;

			return Activator.CreateInstance(m_ScriptFunctions[_function_name]) as IZScriptFunction;
		}
	}
}
