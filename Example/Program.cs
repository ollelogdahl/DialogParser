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
			DialogTree tree = DialogTree.Load("exempel.txt");

			Console.WriteLine(tree.ToString());

			Console.ReadLine();
		}
	}
}
