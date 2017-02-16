using System;
using System.Collections.Generic;
using System.IO;

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
    public static class Writer{
        private static Object aLock = new Object();
        public static void write(string msg){
            lock(aLock){
                Console.WriteLine(msg);
            }
        }
    }

    /*This class controls the main flow.*/
    static class Program{
        static void Main(string[] args)
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

			Zip.createZips(zips);

            //JSONS
            Json version = new Json(enviar, @"version.json");
            Json versionPython = new Json(enviar, @"versionPython.json");

            List<Json> versionFiles = new List<Json>();
            versionFiles.Add(version);
            versionFiles.Add(versionPython);

            Json.IncrementVersionFiles(versionFiles);

            //SINAIS.TXT
            if(File.Exists(Path.Combine(release, "sinais.txt"))){
                Console.WriteLine("\nO arquivo de lista de sinais está presente na pasta release.");
            }else{
                Console.WriteLine("\nATENÇÃO! O arquivo de lista de sinais NÃO está presente na pasta release!");
            }

            Console.WriteLine("Concluído. Aperte alguma tecla para encerrar.");
            Console.ReadKey();
        }
    }
    
}
