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
		public Predicate<string> filter = _ => true;

		/* Creates the zipfile and behaves diferently depending on whether overwrite is true or not.*/
		private void zipFromDirectory()
		{
			Writer.write("Iniciando criação do arquivo:" + this.destinationArchiveFileName + Environment.NewLine + "Sobreescrever arquivo: " + (overwrite ? "Sim." : "Não."));
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
			}
		}


		private static string[] GetEntryNames(string[] names, string sourceFolder, bool includeBaseName)
        {
            if (names == null || names.Length == 0)
                return new string[0];

            if (includeBaseName)
                sourceFolder = Path.GetDirectoryName(sourceFolder);

            int length = string.IsNullOrEmpty(sourceFolder) ? 0 : sourceFolder.Length;
            if (length > 0 && sourceFolder != null &&
            	sourceFolder[length - 1] != Path.DirectorySeparatorChar &&
            	sourceFolder[length - 1] != Path.AltDirectorySeparatorChar)
                length++;

            var result = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                result[i] = names[i].Substring(length);
            }

            return result;
        }


        
		public void CreateFromDirectory()
	    {
	        if (string.IsNullOrEmpty(destinationArchiveFileName)) {
                Writer.write("Nome de arquivo inválido: " + destinationArchiveFileName + ".");
	        }

			Writer.write("Iniciando criação do arquivo:" + this.destinationArchiveFileName + Environment.NewLine + "Sobreescrever arquivo: " + (overwrite ? "Sim." : "Não."));

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
			try
			{
		        var filesToAdd = Directory.GetFiles(sourceDirectoryName, "*", SearchOption.AllDirectories);
		        var entryNames = GetEntryNames(filesToAdd, sourceDirectoryName, true/*includeBaseDirectory*/);
		        using(var zipFileStream = new FileStream(destinationArchiveFileName, FileMode.Create)) {
		            using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create)) {
		                for (int i = 0; i < filesToAdd.Length; i++) {
		                    // Add the following condition to do filtering:
		                    if (!filter(filesToAdd[i]))//.Contains(".git"))
		                    {
		                        continue;
		                    }
		                    archive.CreateEntryFromFile(filesToAdd[i], entryNames[i], 0/*compressionLevel*/);
		                }
		            }
		        }
				Writer.write("Criação do " + this.destinationArchiveFileName + " bem sucedida.");
			}
			catch(Exception ex)
			{
				Writer.write("Erro na criação do zip:\n" + this.destinationArchiveFileName + "\nErro:" + ex + "\nAperte alguma tecla para continuar.");
			}
	    }


		/*Iniciate multiple threads to create the zip files.
        Parameters: List<Zip> List of zip file (not to be confused with ZipFile Class!!).*/
		public static void createZips(List<Zip> zips)
		{
			List<Thread> threads = new List<Thread>();

			foreach (Zip zip in zips)
			{
                threads.Add(new Thread(new ThreadStart(zip.CreateFromDirectory)));//zipFromDirectory)));
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
		public Zip(string sourceDirectoryName, string destinationArchiveFileName, bool overwrite, Predicate<string> filter)
		{
			if (!Directory.Exists(sourceDirectoryName))
			{
				string msg = "O diretório " + sourceDirectoryName + " não existe!\n";
				Writer.write(msg);
				throw new IOException(msg);
			}
			this.sourceDirectoryName = sourceDirectoryName;
			this.destinationArchiveFileName = destinationArchiveFileName;
			this.overwrite = overwrite;
			this.filter = filter;
		}


		/*Constructor without filter*/
		public Zip(string sourceDirectoryName, string destinationArchiveFileName, bool overwrite) : this(sourceDirectoryName, destinationArchiveFileName, overwrite, _ => true)
		{
		}


		/*Constructor overwrite = false (security measure)*/
		public Zip(string sourceDirectoryName, string destinationArchiveFileName) : this(sourceDirectoryName, destinationArchiveFileName, false)
		{
		}
	}
}
