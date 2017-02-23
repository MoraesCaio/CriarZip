using System;

namespace CriarZip
{
	public class Writer
	{
		private static Object aLock = new Object();

		public static void write(string msg)
		{
			lock (aLock)
			{
				Console.WriteLine(msg);
				GUI.textBox1.AppendText(msg + Environment.NewLine);
			}
		}
	}
}
