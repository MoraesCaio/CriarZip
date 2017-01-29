using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

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
            string requisitos = Path.Combine(dir, @"requisitos\");
            string enviar = Path.Combine(dir, @"enviar\");
            string python = Path.Combine(dir, @"python\");
            string release = Path.Combine(dir, @"release\");

            Zip zip_VLIBRAS    = new Zip(VLIBRAS, Path.Combine(enviar, "VLIBRAS.zip"), true);
            Zip zip_requisitos = new Zip(requisitos, Path.Combine(enviar, "requisitos.zip"), true);
            Zip zip_python     = new Zip(python, Path.Combine(release, "python.zip"), true);

            Thread t_VLIBRAS    = new Thread(new ThreadStart(zip_VLIBRAS.zipFromDirectory));
            Thread t_requisitos = new Thread(new ThreadStart(zip_requisitos.zipFromDirectory));
            Thread t_python     = new Thread(new ThreadStart(zip_python.zipFromDirectory));

            t_VLIBRAS.Start();
            t_requisitos.Start();
            t_python.Start();

            Thread.Sleep(1);

            t_VLIBRAS.Join();
            t_requisitos.Join();
            t_python.Join();

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
