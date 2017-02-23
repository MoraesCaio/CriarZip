using System;
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
