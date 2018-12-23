using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DialogParser {

	public class DialogTree {
		public DialogElement[] Elements { get; set; }

		public DialogTree(DialogElement[] e) {
			Elements = e;
		}

		public override string ToString() {
			string s = "";
			for (int i = 0; i < Elements.Length; i++) {
				s += (Elements[i].Name) + Environment.NewLine;
				s += (Elements[i].Text) + Environment.NewLine;
				for (int j = 0; j < Elements[i].Responses.Length; j++) {
					s += Elements[i].Responses[j].ToString() + Environment.NewLine;
				}
				s += Environment.NewLine;
			}
			return s;
		}

		public static DialogTree Load(string path) {
			StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
			string expression = sr.ReadToEnd();
			sr.Close();
			return Parse(expression);
		}

		public static DialogTree Parse(string expression) {
			IEnumerable<Token> tokens = new Tokenizer().Scan(expression);
			Parser parser = new Parser(tokens);
			DialogElement[] dialogs = parser.Parse();

			DialogTree tree = new DialogTree(dialogs);
			tree.AssignResponseIndex();
			return tree;
		}

		private void AssignResponseIndex() {
			foreach(DialogElement e in Elements) {
				foreach(DialogResponse r in e.Responses) {
					for(int i = 0; i < Elements.Length; i++) {
						if(r.TargetName == Elements[i].Name) {
							r.TargetElementIndex = i;
						}
					}
				}
			}
		}
	}

	public class DialogElement {
		public string Name { get; set; }
		public string Text { get; set; }
		public DialogResponse[] Responses { get; set; }

		public DialogElement(string name, string text) {
			Name = name; Text = text;
		}
	}

	public class DialogResponse {
		public string Text { get; set; }
		public string TargetName { get; set; }
		public int TargetElementIndex { get; set; }

		public DialogResponse(string target, string targetName) {
			Text = target;
			TargetName = targetName;
			TargetElementIndex = -1;
		}

		public override string ToString() {
			string s = "";
			s += $" # {Text} {{{TargetElementIndex}}}" + Environment.NewLine;

			return s;
		}
	}
}
