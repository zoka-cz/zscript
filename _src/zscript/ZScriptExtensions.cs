using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Zoka.ZScript
{
	/// <summary>Extensions to the ZScript</summary>
	public static class ZScriptExtensions
	{
		/// <summary>Will add ZServiceFunctionFactory into service collection</summary>
		public static IServiceCollection					AddZService(this IServiceCollection _service_collection)
		{
			_service_collection.AddSingleton<ZScriptFunctionFactory>();
			return _service_collection;
		}
	}
}
