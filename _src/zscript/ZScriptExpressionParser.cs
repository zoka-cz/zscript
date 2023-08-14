using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zoka.ZScript
{
	/// <summary>Parser of ZScript</summary>
	public static class ZScriptExpressionParser
	{
		enum CharacterClass
		{
			AllLetters,
			UpperCaseLetters,
			LowerCaseLetters,
			Numbers,
			UnderScore,
			WhiteSpaces,
			Punctuations,
		}


		/// <summary>Will parse the given text, which may contain the ZScript expressions, which are evaluated and replaced by resulting value</summary>
		public static string								EvaluateScriptReplacements(string _original_text, DataStorages _data_storages, IServiceProvider _service_provider)
		{
			StringBuilder text = new StringBuilder();
			int i = 0;
			while (i < _original_text.Length)
			{
				if (_original_text[i] == '$')
				{
					var expr = ParseScriptExpression(_original_text, ref i);
					var expr_eval = expr.EvaluateExpressionToValue(_data_storages, _service_provider);
					text.Append(expr_eval);
				}
				else
				{
					text.Append(_original_text[i]);
					i++;
				}

			}

			return text.ToString();
		}

		/// <summary>Will parse the ZScript expression (no other text is allows)</summary>
		public static IZScriptExpression						ParseScriptExpression(string _s)
		{
			int pos = 0;
			return ParseScriptExpression(_s, ref pos);
		}

		/// <summary>Will parse the ZScript expression (no other text is allowed) from some start position</summary>
		public static IZScriptExpression						ParseScriptExpression(string _s, ref int _start_position)
		{
			int pos = _start_position;

			if (_s[pos] != '$')
				return ParseLiteralExpression(_s, ref _start_position);
			pos++;

			IZScriptExpression expr = null;

			if (_s[pos] == '$')
			{
				pos--; // revert back by one char, so the parser may check it 
				// it is function only
				expr = ParseFunctionExpression(_s, ref pos);
			}
			else
			{
				// it may be variable or table
				var variable_name = ConsumeCharacters(_s, 
													  ref pos, 
													  new [] { CharacterClass.UnderScore, CharacterClass.AllLetters }, 
													  new [] { CharacterClass.AllLetters, CharacterClass.Numbers, CharacterClass.UnderScore });

				if (pos < _s.Length && _s[pos] == '[')
				{
					var first_indexer = ParseIndexerExpression(_s, ref pos);
					if (pos >= _s.Length || _s[pos] != '[')
					{
						// it is row from table expression
						expr = new TableRowExpression($"${variable_name}", first_indexer);
					}
					else if (_s[pos] == '[')
					{
						var column_indexer = ParseIndexerExpression(_s, ref pos);
						expr = new TableExpression($"${variable_name}", first_indexer, column_indexer);
					}
					else
					{
						throw new FormatException("Could not parse the expression which seemed to be Table or TableRow expression.");
					}
				}
				else
				{
					expr = new VariableExpression($"${variable_name}");
				}
			}
			if (expr == null)
				throw new FormatException("Could not parse the script expression");

			_start_position = pos;
			return expr;
		}


		private static IZScriptExpression					ParseLiteralExpression(string _s, ref int _start_position)
		{
			string literal = null;

			if (_s[_start_position] == '"')
			{
				var sb = new StringBuilder();
				_start_position++;
				while (_start_position < _s.Length)
				{
					if (_s[_start_position] == '"' || (_s[_start_position] == '\\' && _s[_start_position + 1] == '\"'))
					{
						literal = sb.ToString();
						if (_s[_start_position] == '"')
							_start_position++;
						else if (_s[_start_position] == '\\' && _s[_start_position + 1] == '\"')
							_start_position += 2;
						break;
					}
					sb.Append(_s[_start_position]);
					_start_position++;
				}
				if (literal == null)
					throw new FormatException("Literal using quotes must finish with quotation mark");
			}
			else
			{
				literal = ConsumeCharacters(_s, ref _start_position, new[] { CharacterClass.AllLetters, CharacterClass.UnderScore  }, new[] { CharacterClass.AllLetters, CharacterClass.UnderScore, CharacterClass.Numbers, CharacterClass.Punctuations });
			}
			if (string.IsNullOrEmpty(literal))
				throw new FormatException("Literal may not be empty.");
			return new LiteralExpression(literal);
		}

		private static IZScriptExpression					ParseNumericExpression(string _s, ref int _start_possition)
		{
			var numeric = ConsumeCharacters(_s, ref _start_possition, new [] { CharacterClass.Numbers }, new [] { CharacterClass.Numbers });
			if (string.IsNullOrEmpty(numeric))
				throw new FormatException("Numeric may not be written as empty string.");
			int number;
			if (!int.TryParse(numeric, out number))
				throw new FormatException("Numeric is not a number.");

			return new NumericExpression(number);
		}

		private static IZScriptExpression					ParseIndexerExpression(string _s, ref int _start_possition)
		{
			int pos = _start_possition;

			if (_s[pos++] != '[')
				throw new FormatException("Indexer must always start with [ character");

			// skip whitespaces
			ConsumeWhitespaces(_s, ref pos);

			IZScriptExpression indexer;

			var cls = GetCharacterClasses(_s[pos]);
			if (cls.Any(cl => cl == CharacterClass.AllLetters) || _s[pos] == '"')
				indexer = ParseLiteralExpression(_s, ref pos);
			else if (cls.Any(cl => cl == CharacterClass.Numbers))
				indexer = ParseNumericExpression(_s, ref pos);
			else if (_s[pos] == '$')
				indexer = ParseScriptExpression(_s, ref pos);
			else
				throw new FormatException("Table indexer may be only Literal, Numeric or expression");

			// skip whitespaces
			ConsumeWhitespaces(_s, ref pos);

			if (_s[pos++] != ']')
				throw new FormatException($"Indexer must be finished with ] character (near \"{_s.Substring(_start_possition)}\")");

			_start_possition = pos;
			return indexer;
		}

		private static IZScriptExpression					ParseFunctionExpression(string _s, ref int _start_position)
		{
			if (_s[_start_position++] != '$' || _s[_start_position++] != '$')
				throw new FormatException("Function must start with $$ characters.");

			var function_name = ConsumeCharacters(_s, ref _start_position, new [] { CharacterClass.UpperCaseLetters, CharacterClass.UnderScore }, new [] { CharacterClass.UpperCaseLetters, CharacterClass.UnderScore, CharacterClass.Numbers });
			if (string.IsNullOrEmpty(function_name))
				throw new FormatException("Function name must not be empty and may contain only upper case latters, underscore and numbers.");

			if (_s[_start_position++] != '(')
				throw new FormatException("Functions must be called with parameters in parenthesis.");

			var arguments = new List<IZScriptExpression>();

			while (true)
			{
				ConsumeWhitespaces(_s, ref _start_position);

				IZScriptExpression expr;
				var cls = GetCharacterClasses(_s[_start_position]);
				if (cls.Any(cl => cl == CharacterClass.AllLetters) || _s[_start_position] == '"')
					expr = ParseLiteralExpression(_s, ref _start_position);
				else if (cls.Any(cl => cl == CharacterClass.Numbers))
					expr = ParseNumericExpression(_s, ref _start_position);
				else if (_s[_start_position] == '$')
					expr = ParseScriptExpression(_s, ref _start_position);
				else if (_s[_start_position] == ')')
					break;
				else if (_s[_start_position] == '\\' && _s[_start_position + 1] == '\"')
				{
					_start_position++;
					expr = ParseLiteralExpression(_s, ref _start_position);
				}
				
				else 
					throw new FormatException($"Function argument may be only Literal, Numeric or expression");

				arguments.Add(expr);
				ConsumeWhitespaces(_s, ref _start_position);
				if (_s[_start_position] != ')' && _s[_start_position] != ',')
					throw new FormatException($"Function arguments must be closed by ) character and separated by commas (near \"{_s.Substring(_start_position)}\".");
				if (_s[_start_position] == ',')
					_start_position++;
			}

			if (_s[_start_position++] != ')')
				throw new FormatException("Function must end with closing parenthesis");

			return new FunctionExpression(function_name, arguments);
		}

		#region Helpers

		private static void									ConsumeWhitespaces(string _s, ref int _start_position)
		{
			while (_start_position < _s.Length && Char.IsWhiteSpace(_s[_start_position]))
				_start_position++;
		}

		private static string								ConsumeCharacters(string _s, ref int _start_position, CharacterClass[] _first_character_must_be,  CharacterClass [] _character_types_to_consume)
		{
			int pos = _start_position;
			var ret = new StringBuilder();

			if (_first_character_must_be.Any() && !_first_character_must_be.Intersect(GetCharacterClasses(_s[pos])).Any())
				throw new FormatException($"First character is not any from the allowed characters {string.Join(", ", _first_character_must_be)} in string {_s.Substring(_start_position != 0 ? _start_position - 1 : _start_position)}");
			ret.Append(_s[pos++]);

			while (pos < _s.Length)
			{
				var ch_classes = GetCharacterClasses(_s[pos]);
				if (!ch_classes.Intersect(_character_types_to_consume).Any())
					break;

				ret.Append(_s[pos++]);
			}
			
			_start_position = pos;
			return ret.ToString();
		}

		private static IEnumerable<CharacterClass>			GetCharacterClasses(char _c)
		{
			if (Char.IsLetter(_c))
				yield return CharacterClass.AllLetters;
			if (Char.IsUpper(_c))
				yield return CharacterClass.UpperCaseLetters;
			if (Char.IsLower(_c))
				yield return CharacterClass.LowerCaseLetters;
			if (Char.IsNumber(_c))
				yield return CharacterClass.Numbers;
			if (Char.IsWhiteSpace(_c))
				yield return CharacterClass.WhiteSpaces;
			if (Char.IsPunctuation(_c))
				yield return CharacterClass.Punctuations;
			if (_c == '_')
				yield return CharacterClass.UnderScore;

			yield break;
		}

		#endregion // Helpers

	}
}
