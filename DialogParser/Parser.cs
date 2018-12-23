using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DialogParser {
	public class Parser {
		private readonly IEnumerator<Token> tokens;

		public Parser(IEnumerable<Token> t) {
			tokens = t.GetEnumerator();
		}

		public DialogElement[] Parse() {
			tokens.MoveNext();
			List<DialogElement> elements = new List<DialogElement>();

			elements.Add(Dialog());
			while(tokens.Current.GetType() == typeof(DialogBeginToken)) {
				elements.Add(Dialog());

				if (tokens.Current == null) break;
			}

			return elements.ToArray();
		}

		// dialog =	
		//		"[" word "]" 
		//		string
		//		resp { resp }
		public DialogElement Dialog() {

			// acceptera "[" word "]"
			Consume(typeof(DialogBeginToken));
			WordToken dialogNameToken = (WordToken)Consume(typeof(WordToken));
			Consume(typeof(DialogEndToken));

			// accepterar word { " " word }
			string dialogText = String();

			DialogElement dialogElement = new DialogElement(dialogNameToken.Value, dialogText);

			List<DialogResponse> responses = new List<DialogResponse>();
			while (tokens.Current.GetType() == typeof(ResponseToken)) {
				responses.Add(Response());

				if (tokens.Current == null) break;
			}

			dialogElement.Responses = responses.ToArray();
			return dialogElement;
		}

		// resp	=	
		//		"#" string "{" word "}"
		public DialogResponse Response() {
			Consume(typeof(ResponseToken));
			string responseText = String();
			Consume(typeof(ResponseTargetBeginToken));
			WordToken replyTarget = (WordToken)Consume(typeof(WordToken));
			Consume(typeof(ResponseTargetEndToken));

			DialogResponse response = new DialogResponse(responseText, replyTarget.Value);
			return response;
		}

		// scriptable =	"<" word ">"
		public string Scriptable() {
			Consume(typeof(ScriptableBeginToken));

			string script = ((WordToken)Consume(typeof(WordToken))).Value;

			Consume(typeof(ScriptableEndToken));

			return script;
		}

		// string	=	word { word | scriptable }
		public string String() {
			string consumedString = ((WordToken)Consume(typeof(WordToken))).Value;

			while (tokens.Current.GetType() == typeof(WordToken)) {
				consumedString += " " + ((WordToken)Consume(typeof(WordToken))).Value;
			}

			// vi påbörjar ett script
			if(tokens.Current.GetType() == typeof(ScriptableBeginToken)) {
				string script = Scriptable();															// gör nått med denna!

				// fortsätter kolla efter en sträng
				if(tokens.Current.GetType() == typeof(WordToken)) consumedString += " " + String();
			}

			return consumedString;
		}

		public Token Consume(Type t) {
			// kollar om next token är av typ t
			if (tokens.Current.GetType() == t) {
				// gå till nästa
				Token result = tokens.Current;
				tokens.MoveNext();

				return result;
			} else throw new Exception($"Parser consumed {tokens.Current.GetType()}, expected '{t}'");
		}
	}
}
