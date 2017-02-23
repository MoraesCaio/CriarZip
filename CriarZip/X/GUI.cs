using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CriarZip
{
	public static class Writer
	{
		private static Object aLock = new Object();
		public static void write(string msg)
		{
			lock (aLock)
			{
				Console.WriteLine(msg);
			}
		}
	}

	public partial class GUI : Form
	{


		public GUI()
		{
			InitializeComponent();
		}

		public void AppendTextBox(string value)
		{
			if (InvokeRequired)
			{
				this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
				return;
			}
			textBox1.Text += value;
		}

		void SampleFunction()
		{
			// Gets executed on a seperate thread and 
			// doesn't block the UI while sleeping
			for (int i = 0; i < 5; i++)
			{
				AppendTextBox("hi.  ");
				Thread.Sleep(1000);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//FOLDERS
			string dir = Directory.GetCurrentDirectory();
			string VLIBRAS = Path.Combine(dir, @"VLIBRAS\");
			string enviar = Path.Combine(dir, @"enviar\");
			string python = Path.Combine(dir, @"python\");
			string release = Path.Combine(dir, @"release\");

			//ZIPS
			List<Zip> zips = new List<Zip>();
			zips.Add(new Zip(VLIBRAS, Path.Combine(enviar, "VLIBRAS.zip"), true));
			zips.Add(new Zip(python, Path.Combine(enviar, "python.zip"), true));

			//JSONS
			List<Json> versionFiles = new List<Json>();
			versionFiles.Add(new Json(enviar, @"version.json"));
			versionFiles.Add(new Json(enviar, @"versionPython.json"));
			
			Zip.createZips(zips);

			Json.IncrementVersionFiles(versionFiles);

			//SINAIS.TXT
			if (File.Exists(Path.Combine(release, "sinais.txt")))
			{
				Writer.write("\nO arquivo de lista de sinais está presente na pasta release.");
			}
			else
			{
				Writer.write("\nATENÇÃO! O arquivo de lista de sinais NÃO está presente na pasta release!");
			}

			Writer.write("Concluído. Aperte alguma tecla para encerrar.");
			Console.ReadKey();
		}
	}
}
