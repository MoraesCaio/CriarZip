using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace CriarZip
{
	/* This class is meant to be used when working with json files following the format:
	 *	{
	 *		"Major":1,
	 *		"Minor":33,
	 *		"Build":0,
	 *		"Revision":11
	 *	}
	 *	The indentation is not a problem. However, when writing files, it does not maintains it indented.
	 *	An instance of it represents a json file.
	 * Author: Caio Moraes
     * GitHub: MoraesCaio
     * email:  caiomoraes@msn.com
	 **/

	public class Json
	{
		/*The file and it's path (the path does not contain the file name!)*/
		public string path;
		public string fileName;
		/*Properties with the new version to be saved*/
		public int major;
		public int minor;
		public int build;
		public int revision;

		private StreamReader sr;
		private string version;
		private dynamic arrayVersion;
		private JavaScriptSerializer serializer = new JavaScriptSerializer();

		/*Following the Visual Studio's sugestion, this method increment the Revision value.
		Parameters: List<Json> list of json version files*/
		public static void IncrementVersionFiles(List<Json> versionFiles)
		{
			foreach (Json versionFile in versionFiles)
			{
				versionFile.Read();
				versionFile.IncrementRevision(1);
				versionFile.Write();
				versionFile.PrintVersion();
			}
		}

		/*Constructor*/
		public Json(string path, string fileName)
		{
			if (!File.Exists(Path.Combine(path, fileName)))
			{
				throw new System.IO.IOException("O arquivo " + fileName + " não existe no diretório " + path + ".\n");
			}
			this.path = path;
			this.fileName = fileName;
		}

		/*Read the json file and stores it's version atribute's values.*/
		public void Read()
		{
			sr = new StreamReader(Path.Combine(path, fileName));
			version = sr.ReadToEnd();
			sr.Close();
			arrayVersion = serializer.DeserializeObject(version);
			major = Convert.ToInt32(arrayVersion["Major"]);
			minor = Convert.ToInt32(arrayVersion["Minor"]);
			build = Convert.ToInt32(arrayVersion["Build"]);
			revision = Convert.ToInt32(arrayVersion["Revision"]);
		}

		/*Methods for incrementing version's values.*/
		public void IncrementMajor(int i)
		{
			arrayVersion["Major"] += i;
			major = Convert.ToInt32(arrayVersion["Major"]);
		}
		public void IncrementMinor(int i)
		{
			arrayVersion["Minor"] += i;
			minor = Convert.ToInt32(arrayVersion["Minor"]);
		}
		public void IncrementBuild(int i)
		{
			arrayVersion["Build"] += i;
			build = Convert.ToInt32(arrayVersion["Build"]);
		}
		public void IncrementRevision(int i)
		{
			arrayVersion["Revision"] += i;
			revision = Convert.ToInt32(arrayVersion["Revision"]);
		}
		/*Write's the file with the new version.*/
		public void Write()
		{
			string novaVersao = serializer.Serialize(arrayVersion);
			File.WriteAllText(Path.Combine(path, fileName), novaVersao);
		}

		public void PrintVersion()
		{
			Console.WriteLine("\n{0}", fileName);
			Console.WriteLine("Major: {0}", major);
			Console.WriteLine("Minor: {0}", minor);
			Console.WriteLine("Build: {0}", build);
			Console.WriteLine("Revision: {0}", revision);
		}
	}
}
