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
            Json version = new Json(enviar, @"version.json");
            Json versionPython = new Json(enviar, @"versionPython.json");

            List<Json> versionFiles = new List<Json>();
            versionFiles.Add(version);
            versionFiles.Add(versionPython);

            foreach(Json versionFile in versionFiles){
                versionFile.Read();
                versionFile.IncrementRevision(1);
                versionFile.Write();
                Console.WriteLine("\n{0}",versionFile.fileName);
                Console.WriteLine("Major: {1}",versionFile.major);
                Console.WriteLine("Minor: {2}",versionFile.minor);
                Console.WriteLine("Build: {3}",versionFile.build);
                Console.WriteLine("Revision: {4}", versionFile.revision);
            }

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
    public class Json{
        public string path;
        public string fileName;
        public int major;
        public int minor;
        public int build;
        public int revision;

        private StreamReader sr;
        private string version;
        private dynamic arrayVersion;
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public Json(string path, string fileName){
            if(!File.Exists(Path.Combine(path, fileName))){
                throw new System.IO.IOException("O arquivo " + fileName + " não existe no diretório " + path + ".\n");
            }
            this.path = path;
            this.fileName = fileName;
        }

        public void Read(){
            sr = new StreamReader(Path.Combine(path, fileName));
            version = sr.ReadToEnd();
            sr.Close();
            arrayVersion = serializer.DeserializeObject(version);
            major = Convert.ToInt32(arrayVersion["Major"]);
            minor = Convert.ToInt32(arrayVersion["Minor"]);
            build = Convert.ToInt32(arrayVersion["Build"]);
            revision = Convert.ToInt32(arrayVersion["Revision"]);
        }

        public void IncrementMajor(int i){
            arrayVersion["Major"] += i;
            major = Convert.ToInt32(arrayVersion["Major"]);
        }
        public void IncrementMinor(int i){
            arrayVersion["Minor"] += i;
            minor = Convert.ToInt32(arrayVersion["Minor"]);
        }
        public void IncrementBuild(int i){
            arrayVersion["Build"] += i;
            build = Convert.ToInt32(arrayVersion["Build"]);
        }
        public void IncrementRevision(int i){
            arrayVersion["Revision"] += i;
            revision = Convert.ToInt32(arrayVersion["Revision"]);
        }

        public void Write(){
            string novaVersao = serializer.Serialize(arrayVersion);
            File.WriteAllText(Path.Combine(path, fileName), novaVersao);
        }
    }
}
