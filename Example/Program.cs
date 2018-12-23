using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DialogParser;

namespace Example {
	class Program {
		static void Main(string[] args) {
			DialogTree tree = DialogTree.Load("exempel.dlg");

			string expression = @"
[Start]
Hej här börjar alltid alla program
# Okej {Ok}
# Varför {Why}

[Ok]
Här ska programmet sluta
# Hejdå {End}
				";
			var tokens = new Tokenizer().Scan(expression);
			var parser = new Parser(tokens);
		}
	}
}
