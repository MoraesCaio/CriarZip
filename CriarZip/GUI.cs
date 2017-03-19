using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

/*CriarZip is intended for:
 *  - creating the Zip Files (multithreading),
 *  - incrementing the version files,
 *  - check if the file sinais.txt exists.
 *  Author: Caio Moraes
 *  GitHub: MoraesCaio
 *  email:  caiomoraes@msn.com
 **/
namespace CriarZip
{
	public partial class GUI : Form
	{

		public static TextBox textBox1 = new TextBox();

		public GUI()
		{
			InitializeComponent();
			textBox1.Dock = DockStyle.Fill;
			textBox1.Location = new System.Drawing.Point(3, 3);
			textBox1.Multiline = true;
			textBox1.Name = "textBox1";
			textBox1.ReadOnly = true;
			textBox1.Size = new System.Drawing.Size(637, 274);
			textBox1.TabIndex = 0;
			textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
			tableLayoutPanel1.Controls.Add(textBox1, 0, 0);
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


		private void button1_Click(object sender, EventArgs e)
		{
			textBox1.Text = "";

			//FOLDERS
            //"..\Deploy-VLibras"
			string dir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, @"Deploy-VLibras\");
            string curDir = Directory.GetCurrentDirectory();
            string VLIBRAS = Path.Combine(dir, @"VLibras\");
			string python = Path.Combine(dir, @"Python-portable\");
			string enviar = Path.Combine(curDir, @"enviar\");
			string release = Path.Combine(curDir, @"release\");
            Predicate<string> filterZip = fileName => !fileName.Contains(".git");

            //ZIPS
            List<Zip> zips = new List<Zip>();
			zips.Add(new Zip(VLIBRAS, Path.Combine(enviar, "VLIBRAS.zip"), true, filterZip));
			zips.Add(new Zip(python, Path.Combine(enviar, "python.zip"), true, filterZip));

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

			Writer.write("Concluído.");
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
