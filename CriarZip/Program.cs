using System;
using System.Windows.Forms;

namespace CriarZip
{

	/*This class controls the main flow.*/
	static class Program
	{
		[STAThread]
		static void Main(string[] args)
        {
			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new GUI());
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
        }
    }

}
