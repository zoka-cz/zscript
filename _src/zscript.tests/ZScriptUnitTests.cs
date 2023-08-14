using Zoka.ZScript;

namespace zscript.tests
{
	[TestClass]
	public class ZScriptUnitTests
	{
		[TestMethod]
		public void											AtSignIsParsedAsPartOfLiteral()
		{
			var src = "testemail@testdomain.cz";
			DataStorages dt = new();
			

			var tgt = ZScriptExpressionParser.ParseScriptExpression(src).EvaluateExpressionToValue(dt, null);

			Assert.IsInstanceOfType(tgt, typeof(string));
			Assert.AreEqual(src, tgt);
		}
	}
}