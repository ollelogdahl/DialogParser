using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogueParser {

	public class DialogueTree {
		DialogueElement[] Elements { get; set; }

		public DialogueTree(DialogueElement[] e) {
			Elements = e;
		}

		public DialogueTree Load(string path) {
			List<DialogueElement> e = new List<DialogueElement>();

			StreamReader reader = new StreamReader(path);
			string all = reader.ReadToEnd();

			return new DialogueTree(e.ToArray());
		}
	}

	public class DialogueElement {
		string Text { get; set; }
		DialogueResponse[] Responses { get; set; }

		public DialogueElement(string s) {

		}
	}

	public class DialogueResponse {
		string Text { get; set; }
		DialogueElement TargetElement { get; set; }

	}
}
