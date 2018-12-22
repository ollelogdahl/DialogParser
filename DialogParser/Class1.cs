using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogParser {

	public class DialogTree {
		DialogElement[] Elements { get; set; }

		public DialogTree(DialogElement[] e) {
			Elements = e;
		}

		public DialogTree Load(string path) {
			List<DialogElement> e = new List<DialogElement>();

			string[] lines = File.ReadAllLines(path);

			for(int i = 0; i < lines.Length; i++) {
				string line = lines[i];

				if(line[0] == '[') {
					
				}
			}

			return new DialogTree(e.ToArray());
		}
	}

	public class DialogElement {
		string Text { get; set; }
		DialogResponse[] Responses { get; set; }

		public DialogElement(string s) {

		}
	}

	public class DialogResponse {
		string Text { get; set; }
		DialogElement TargetElement { get; set; }

	}
}
