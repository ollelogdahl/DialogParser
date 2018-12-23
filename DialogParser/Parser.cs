using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogParser {
	public abstract class Token {
	}

	public class DialogBeginToken : Token {

	}
	public class DialogEndToken : Token {

	}

	public class StringToken : Token {
		public string Value { get; }

		public StringToken(string value) {
			Value = value;
		}
	}
	public class StringEndToken : Token {
	}

	public class ResponseToken : Token {

	}

	public class ResponseTargetBeginToken : Token {
	}
	public class ResponseTargetEndToken : Token {
	}

	// läser en sekvens av tecken, och minimerar dess innehåll till tokens
	public class Tokenizer {
		private StringReader reader;


		// Grammatik regler ( EBNF )
		// start	=	{dialog}
		// dialog	=	"[" ident "]" 
		//				string
		//				response
		//				{ response }
		// response =	"#" string "{" ident "}"
		
		// ident	=	letter { letter }
		// string	=	alphanumeric { alphanumeric | punct | separator }
		public IEnumerable<Token> Scan(string sequence) {
			reader = new StringReader(sequence);

			var tokens = new List<Token>();
			while(reader.Peek() != -1) {

				// hoppar över tomma rader när vi letar efter tokens, vi 
				// kan alltid lägga till tomma rader och mellanrum (mellan tokens).
				while (Char.IsWhiteSpace((char)reader.Peek())) reader.Read();
				if (reader.Peek() == -1) break;

				// läser ett tecken i taget och ifall tecknet är en av våra
				// Terminal-symboler lägger vi till dess token till listan.
				// (https://en.wikipedia.org/wiki/Lexical_analysis#Token)
				char c = (char)reader.Peek();
				switch(c) {
					case '[':
						tokens.Add(new DialogBeginToken());
						reader.Read();
						break;
					case ']':
						tokens.Add(new DialogEndToken());
						reader.Read();
						break;
					case '#':
						tokens.Add(new ResponseToken());
						reader.Read();
						break;
					case '{':
						tokens.Add(new ResponseTargetBeginToken());
						reader.Read();
						break;
					case '}':
						tokens.Add(new ResponseTargetEndToken());
						reader.Read();
						break;
					case '\n':
						tokens.Add(new StringEndToken());
						reader.Read();
						break;
					default:
						if (Char.IsLetterOrDigit(c)) {
							string s = ParseString();
							tokens.Add(new StringToken(s));
						} else {
							string remaining = reader.ReadToEnd() ?? string.Empty;
							throw new Exception($"Unknown grammar {c} | {sequence.Length - remaining.Length} : '{remaining}'");
						}
						break;
				}
			}

			return tokens;
		}

		private string ParseString() {
			var chars = new List<char>();
			while (Char.IsLetterOrDigit((char)reader.Peek()) | Char.IsSeparator((char)reader.Peek())) {
				char c = (char)reader.Read();
				chars.Add(c);
			}

			return new string(chars.ToArray());
		}
	}
	public class Parser {
		private readonly IEnumerator<Token> tokens;

		public Parser(IEnumerable<Token> t) {
			tokens = t.GetEnumerator();
		}

		public DialogElement[] Parse() {
			tokens.MoveNext();

			List<DialogElement> elements = new List<DialogElement>();
			while(tokens.Current.GetType() == typeof(DialogBeginToken)) {
				elements.Add(Dialog());
			}

			return elements.ToArray();
		}

		public DialogElement Dialog() {
			DialogElement e = new DialogElement();

			Consume(typeof(DialogBeginToken));
			StringToken tokenName = (StringToken)Consume(typeof(StringToken));
			Consume(typeof(DialogEndToken));

			StringToken tokenText = (StringToken)Consume(typeof(StringToken));
			Consume(typeof(StringEndToken));

			// konsumera resten.-.---...
			List<DialogResponse> responses = new List<DialogResponse>();
			while (tokens.Current.GetType() == typeof(ResponseToken)) {
				responses.Add(Response());

				if (tokens.Current == null) break;
			}

			e.Name = tokenName.Value;
			e.Text = tokenText.Value;
			e.Responses = responses.ToArray();
			return e;
		}

		public DialogResponse Response() {
			Consume(typeof(ResponseToken));
			StringToken replyText = (StringToken)Consume(typeof(StringToken));
			Consume(typeof(ResponseTargetBeginToken));
			StringToken replyTarget = (StringToken)Consume(typeof(StringToken));
			Consume(typeof(ResponseTargetEndToken));

			DialogResponse response = new DialogResponse();
			response.Text = replyText.Value;
			// response.TargetElementIndex = replyTarget.Value;		fixa denna, 
			return response;
		}

		public Token Consume(Type t) {
			// kollar om next token är av typ t
			if(tokens.Current.GetType() == t) {
				// gå till nästa
				Token result = tokens.Current;
				tokens.MoveNext();

				return result;
			}
			return null;
		}
	}
}
