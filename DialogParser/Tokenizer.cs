using System;
using System.Collections.Generic;
using System.IO;

namespace DialogParser {
	// läser en sekvens av tecken, och minimerar dess innehåll till tokens
	public class Tokenizer {
		private StringReader reader;


		// Grammatik regler ( EBNF )
		// start	=	{dialog}
		// dialog	=	"[" word "]" 
		//				string
		//				response
		//				{ response }
		// response =	"#" string "{" word "}"

		// ident	=	letter { letter }
		// word		=	alphanumeric { alphanumeric | punct | separator }
		// string	=	word { word [ scriptable ] }
		// scriptable =	"<" word ">"
		public IEnumerable<Token> Scan(string sequence) {
			reader = new StringReader(sequence);

			var tokens = new List<Token>();
			while (reader.Peek() != -1) {

				// hoppar över tomma rader när vi letar efter tokens, vi 
				// kan alltid lägga till tomma rader och mellanrum (mellan tokens).
				while (Char.IsWhiteSpace((char)reader.Peek())) reader.Read();
				if (reader.Peek() == -1) break;

				tokens.Add( ScanToken((char)reader.Peek(), sequence));
			}

			return tokens;
		}

		// läser ett tecken i taget och ifall tecknet är en av våra
		// Terminal-symboler lägger vi till dess token till listan.
		// (https://en.wikipedia.org/wiki/Lexical_analysis#Token)
		private Token ScanToken(char c, string sequence) {
			Token t;

			switch (c) {
				case '[':
					t = new DialogBeginToken();
					reader.Read();
					break;
				case ']':
					t = new DialogEndToken();
					reader.Read();
					break;
				case '#':
					t = new ResponseToken();
					reader.Read();
					break;
				case '{':
					t = new ResponseTargetBeginToken();
					reader.Read();
					break;
				case '}':
					t = new ResponseTargetEndToken();
					reader.Read();
					break;
				case '<':
					t = new ScriptableBeginToken();
					reader.Read();
					break;
				case '>':
					t = new ScriptableEndToken();
					reader.Read();
					break;
				default:
					t = IsValidWordToken(sequence, c);
					break;
			}

			return t;
		}

		// läser tecknet ifall det inte är nån av terminalera
		// Ifall tecknet inte är en bokstav/siffra, ge error
		private Token IsValidWordToken(string sequence, char c) {
			if (Char.IsLetterOrDigit(c)) {
				string s = ScanString();
				return new WordToken(s);
			} else {
				string remaining = reader.ReadToEnd() ?? string.Empty;
				throw new Exception($"Unknown grammar '{c}' at {sequence.Length - remaining.Length}");
			}
		}

		private string ScanString() {
			var chars = new List<char>();
			while (Char.IsLetterOrDigit((char)reader.Peek()) | IsPunct((char)reader.Peek())) {
				char c = (char)reader.Read();
				chars.Add(c);
			}

			return new string(chars.ToArray());
		}


		private bool IsPunct(char v) {
			switch(v) {
				case '!':
				case '"':
				case '#':
				case '%':
				case '&':
				case '\'':
				case '(':
				case ')':
				case '*':
				case ',':
				case '-':
				case '.':
				case '/':
				case ':':
				case ';':
				case '?':
				case '@':
				case '\\':
				case '_':
					return true;
				default:
					return false;
			}
		}
	}
}
