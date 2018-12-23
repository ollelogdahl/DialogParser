using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DialogParser {

	public class DialogTree {
		DialogElement[] Elements { get; set; }

		public DialogTree(DialogElement[] e) {
			Elements = e;
		}

		public static DialogTree Load(string path) {
			List<DialogElement> e = new List<DialogElement>();

			string[] lines = File.ReadAllLines(path);

			DialogElement current = new DialogElement();
			List<DialogResponse> currentResponses = new List<DialogResponse>();



			for (int i = 0; i < lines.Length; i++) {
				if (String.IsNullOrEmpty(lines[i])) continue;

				// vi påbörjar en ny DialogElement
				if (lines[i][0] == '[') {
					if (current.Name != null) {
						current.Responses = currentResponses.ToArray();
						currentResponses.Clear();

						e.Add(current);   // lägger till föregående ifall finns
					}

					current = new DialogElement();
					current.Name = lines[i].Substring(1, lines[i].Length - 2);
					continue;
				}

				// fördröjd text till dialog rutan
				if (lines[i][0] == '=') {
					current.Text += " " + lines[i];

					continue;
				}

				// svar
				if (lines[i][0] == '#') {
					DialogResponse r = new DialogResponse();
					r.Text = lines[i].Substring(2);		//tar bort "# "
					currentResponses.Add(r);

					continue;
				}

				// text till dialog rutan
				if (!currentResponses.Any()) {
					current.Text += lines[i];
				}
			}

			if (current.Name != null) {
				current.Responses = currentResponses.ToArray();
				e.Add(current);
			}


			// fixa alla indexpexare i responses
			foreach(DialogElement element in e) {
				foreach(DialogResponse r in element.Responses) {
					string targetName = Regex.Match(r.Text, @"\{([^)]*)\}").Groups[1].Value;
					for(int i = 0; i < e.Count; i++) {
						if (e[i].Name == targetName) r.TargetElementIndex = i;
					}

					r.Text = r.Text.Substring(0, r.Text.IndexOf('{'));
				}
			}

			return new DialogTree(e.ToArray());
		}
	}

	public class DialogElement {
		public string Name { get; set; }
		public string Text { get; set; }
		public DialogResponse[] Responses { get; set; }
	}

	public class DialogResponse {
		public string Text { get; set; }
		public int TargetElementIndex { get; set; }

	}
}
