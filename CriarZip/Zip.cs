using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading;

namespace CriarZip
{
	/* This class is meant to be an easy way to create multiple zip file using multithread.
	 * An instance represents a ZipFile, it's file can be overwrited if (bool) overwrite is set true.
	 * Author: Caio Moraes
     * GitHub: MoraesCaio
     * email:  caiomoraes@msn.com
	 **/
	public class Zip
	{
		public string sourceDirectoryName;
		public string destinationArchiveFileName;
		public bool overwrite;

		/* Creates the zipfile and behaves diferently depending on whether overwrite is true or not.*/
		private void zipFromDirectory()
		{
			string msg = "Iniciando criação do arquivo:\n" + this.destinationArchiveFileName + "\nSobreescrever arquivo: " + (overwrite ? "Sim." : "Não.") + "\n";
			Writer.write(msg);
			try
			{
				if (File.Exists(this.destinationArchiveFileName))
				{
					if (overwrite)
					{
						File.Delete(this.destinationArchiveFileName);
					}
					else
					{
						Writer.write("Arquivo " + this.destinationArchiveFileName + " já existe e não será sobreescrito.");
						return;
					}
				}
				ZipFile.CreateFromDirectory(this.sourceDirectoryName, this.destinationArchiveFileName);
				Writer.write("Criação do " + this.destinationArchiveFileName + " bem sucedida.");
			}
			catch (Exception e)
			{
				Writer.write("Erro na criação do zip:\n" + this.destinationArchiveFileName + "\nErro:" + e + "\nAperte alguma tecla para continuar.");
				Console.ReadKey();
			}
		}

		/*Iniciate multiple threads to create the zip files.
        Parameters: List<Zip> List of zip file (not to be confused with ZipFile Class!!).*/
		public static void createZips(List<Zip> zips)
		{
			List<Thread> threads = new List<Thread>();

			foreach (Zip zip in zips)
			{
				threads.Add(new Thread(new ThreadStart(zip.zipFromDirectory)));
			}
			foreach (Thread thread in threads)
			{
				thread.Start();
			}
			Thread.Sleep(1);
			foreach (Thread thread in threads)
			{
				thread.Join();
			}
		}

		/*Full constructor*/
		public Zip(string sourceDirectoryName, string destinationArchiveFileName, bool overwrite)
		{
			if (!Directory.Exists(sourceDirectoryName))
			{
				throw new System.IO.IOException("O diretório " + sourceDirectoryName + " não existe!\n");
			}
			this.sourceDirectoryName = sourceDirectoryName;
			this.destinationArchiveFileName = destinationArchiveFileName;
			this.overwrite = overwrite;
		}

		/*Constructor overwrite = false (security measure)*/
		public Zip(string sourceDirectoryName, string destinationArchiveFileName)
		{
			if (!Directory.Exists(sourceDirectoryName))
			{
				throw new System.IO.IOException("O diretório " + sourceDirectoryName + " não existe!\n");
			}
			this.sourceDirectoryName = sourceDirectoryName;
			this.destinationArchiveFileName = destinationArchiveFileName;
			this.overwrite = false;
		}
	}
}
