using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

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
    public class Zip{
        public string sourceDirectoryName;
        public string destinationArchiveFileName;
        public bool overwrite;

        public void zipFromDirectory(){
            string msg = "Iniciando criação do arquivo:\n"+this.destinationArchiveFileName+"\nSobreescrever arquivo: " + (overwrite? "Sim.":"Não.")+"\n";
            Writer.write(msg);
            try{
                if(File.Exists(this.destinationArchiveFileName)){
                    if(overwrite){
                        File.Delete(this.destinationArchiveFileName);
                    }else{
                        return;
                    }
                }
                ZipFile.CreateFromDirectory(this.sourceDirectoryName, this.destinationArchiveFileName);
                Writer.write("Criação do "+this.destinationArchiveFileName+" bem sucedida.");
            }catch(Exception e){
                Writer.write("Erro na criação do zip:\n"+this.destinationArchiveFileName+"\nErro:"+e+"\nAperte alguma tecla para continuar.");
                Console.ReadKey();
            }
        }

        public Zip(string sourceDirectoryName, string destinationArchiveFileName, bool overwrite){
            this.sourceDirectoryName = sourceDirectoryName;
            this.destinationArchiveFileName = destinationArchiveFileName;
            this.overwrite = overwrite;
        }
        public Zip(string sourceDirectoryName, string destinationArchiveFileName){
            this.sourceDirectoryName = sourceDirectoryName;
            this.destinationArchiveFileName = destinationArchiveFileName;
            this.overwrite = false;
        }
    }
    static class Program{
        static void Main(string[] args)
        {
            //PASTAS
            string dir = Directory.GetCurrentDirectory();
            string VLIBRAS = Path.Combine(dir, @"VLIBRAS\");
            //string requisitos = Path.Combine(dir, @"requisitos\");
            string enviar = Path.Combine(dir, @"enviar\");
            string python = Path.Combine(dir, @"python\");
            string release = Path.Combine(dir, @"release\");

            List<Zip> zips = new List<Zip>();
            zips.Add(new Zip(VLIBRAS, Path.Combine(enviar, "VLIBRAS.zip"), true));
            zips.Add(new Zip(python, Path.Combine(enviar, "python.zip"), true));

            List<Thread> threads = new List<Thread>();
            foreach(Zip zip in zips){
                threads.Add(new Thread(new ThreadStart(zip.zipFromDirectory)));
            }
            foreach(Thread thread in threads){
                thread.Start();
            }
            Thread.Sleep(1);
            foreach(Thread thread in threads){
                thread.Join();
            }
            
            //version.json e versionPython.json
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            StreamReader sr = new StreamReader(Path.Combine(enviar,@"version.json"));
            string version = sr.ReadToEnd();
            dynamic arrayVersion = serializer.DeserializeObject(version);
            arrayVersion["Revision"] += 1;
            string novoArrayVersion = serializer.Serialize(arrayVersion);
            sr.Close();
            File.WriteAllText(Path.Combine(enviar,@"version.json"), novoArrayVersion);

            sr = new StreamReader(Path.Combine(enviar,@"versionPython.json"));
            string pythonJson = sr.ReadToEnd();
            dynamic arrayPython = serializer.DeserializeObject(pythonJson);
            arrayPython["Revision"] += 1;
            string novoArrayPython = serializer.Serialize(arrayPython);
            sr.Close();
            File.WriteAllText(Path.Combine(enviar,@"versionPython.json"), novoArrayPython);

            int vMaj = Convert.ToInt32(arrayVersion["Major"]);
            int vMin = Convert.ToInt32(arrayVersion["Minor"]);
            int vBui = Convert.ToInt32(arrayVersion["Build"]);
            int vRev = Convert.ToInt32(arrayVersion["Revision"]);

            int pMaj = Convert.ToInt32(arrayPython["Major"]);
            int pMin = Convert.ToInt32(arrayPython["Minor"]);
            int pBui = Convert.ToInt32(arrayPython["Build"]);
            int pRev = Convert.ToInt32(arrayPython["Revision"]);

            Console.WriteLine("\nversion.json\nMajor: {0}\nMinor: {1}\nBuild: {2}\nRevision: {3}", vMaj, vMin, vBui, vRev);
            Console.WriteLine("\nversionPython.json\nMajor: {0}\nMinor: {1}\nBuild: {2}\nRevision: {3}", pMaj, pMin, pBui, pRev);

            //sinais
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
