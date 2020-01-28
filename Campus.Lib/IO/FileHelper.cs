using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Campus.Lib.IO
{
    public class FileHelper
    {
        public string _caminhoArquivoBase { get; protected set; }

        public FileHelper(string caminhoArquivoBase)
        {
            _caminhoArquivoBase = caminhoArquivoBase;

            if (!Directory.Exists(caminhoArquivoBase))  
            {
                Directory.CreateDirectory(caminhoArquivoBase);
            }
        }

        public FileStream ObterArquivo(string caminho)
        {
            var arquivo = Path.Combine(_caminhoArquivoBase, caminho);
            return new FileStream(arquivo, FileMode.Open);
        }

        public virtual void GravarArquivo(byte[] arquivo, string caminhoArquivo)
        {
            var caminho = Path.Combine(_caminhoArquivoBase, caminhoArquivo);
            using (StreamWriter streamWriter = new StreamWriter(caminho, true))
            {
                streamWriter.BaseStream.Write(arquivo, 0, arquivo.Length);
            }
        }

        public void ApagarArquivo(string caminho)
        {
            caminho = Path.Combine(_caminhoArquivoBase, caminho);
            if (File.Exists(caminho))
            {
                File.Delete(caminho);
            }
        }
    }
}
