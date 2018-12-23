using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogParser {
	public abstract class Token {}

	public class DialogBeginToken : Token {}
	public class DialogEndToken : Token {}

	public class WordToken : Token {
		public string Value { get; }

		public WordToken(string value) {
			Value = value;
		}
	}

	public class ResponseToken : Token {}

	public class ScriptableBeginToken : Token {}
	public class ScriptableEndToken : Token {}

	public class ResponseTargetBeginToken : Token {}
	public class ResponseTargetEndToken : Token {}
}
