using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campus.Lib.IO
{
    public class HelpZip
    {

        public static void ZipFile(string pathOrigem, string pathDestino, bool manterHierarquiaPastas)
        {
            string[] lstfilenames = new string[] { };

            if (Directory.Exists(pathOrigem))
                lstfilenames = Directory.GetFiles(pathOrigem);
            else if (File.Exists(pathOrigem))
                lstfilenames = new String[] { pathOrigem };
            else
                throw new FileNotFoundException("Origem não encontrada ou vazia (se for um diretorio, ocasiona erro).", pathDestino);

            ZipFile(lstfilenames, pathDestino, manterHierarquiaPastas);
        }

        public static Stream ZipFile(string[] lstfilenames, string pathDestino)
        {
            return ZipFile(lstfilenames, pathDestino, false);
        }

        private static Stream ZipFile(string[] lstfilenames, string pathDestino, bool manterHierarquiaPastas)
        {
            string lNovoNome;
            Crc32 lcrc = new Crc32();
            FileStream lFileDestino = File.Create(pathDestino);
            ZipOutputStream lZipOutputStream = new ZipOutputStream(lFileDestino);

            lZipOutputStream.SetLevel(9); // 0 - store only to 9 - means best compression
            try
            {

                foreach (string lfile in lstfilenames)
                {

                    FileStream lfs = File.OpenRead(lfile);

                    byte[] buffer = new byte[lfs.Length];
                    lfs.Read(buffer, 0, buffer.Length);
                    if (!manterHierarquiaPastas)
                    {
                        FileInfo lFileInfo = new FileInfo(lfile);
                        lNovoNome = lFileInfo.Name;
                    }
                    else
                    {
                        lNovoNome = lfile;
                    }

                    ZipEntry lentry = new ZipEntry(lNovoNome);

                    lentry.DateTime = DateTime.Now;

                    // set Size and the crc, because the information
                    // about the size and crc should be stored in the header
                    // if it is not set it is automatically written in the footer.
                    // (in this case size == crc == -1 in the header)
                    // Some ZIP programs have problems with zip files that don't store
                    // the size and crc in the header.
                    lentry.Size = lfs.Length;
                    lfs.Close();

                    lcrc.Reset();
                    lcrc.Update(buffer);

                    lentry.Crc = lcrc.Value;

                    lZipOutputStream.PutNextEntry(lentry);

                    lZipOutputStream.Write(buffer, 0, buffer.Length);

                }
            }
            finally
            {
                lZipOutputStream.Finish();
                lZipOutputStream.Close();
            }
            return lZipOutputStream;
        }

        /// <summary>
        /// Zipa um Arquivo ou um diretorio
        /// </summary>
        /// <param name="pOrigem">Informa um arquivo ou diretorio a ser zipado</param>
        /// <param name="pArquivoDestino">Informa o nome do arquivo zipado</param>
        public static void ZipFile(string pOrigem, string pArquivoDestino)
        {
            ZipFile(pOrigem, pArquivoDestino, false);
        }


        public static Stream ZipFile(Stream dadosOrigem, string pathOrigem, string pathDestino)
        {
            Crc32 lcrc = new Crc32();
            System.IO.MemoryStream lFileDestino = new MemoryStream();

            ZipOutputStream lZipOutputStream = new ZipOutputStream(lFileDestino);
            lZipOutputStream.SetLevel(9); // 0 - store only to 9 - means best compression
            try
            {
                ZipEntry lentry = new ZipEntry(pathOrigem);
                lentry.DateTime = DateTime.Now;

                // set Size and the crc, because the information
                // about the size and crc should be stored in the header
                // if it is not set it is automatically written in the footer.
                // (in this case size == crc == -1 in the header)
                // Some ZIP programs have problems with zip files that don't store
                // the size and crc in the header.

                byte[] buffer = new byte[dadosOrigem.Length];
                dadosOrigem.Read(buffer, 0, buffer.Length);


                lentry.Size = buffer.Length;

                lcrc.Reset();
                lcrc.Update(buffer);
                lentry.Crc = lcrc.Value;
                lZipOutputStream.PutNextEntry(lentry);
                lZipOutputStream.Write(buffer, 0, buffer.Length);
                lZipOutputStream.CloseEntry();
            }
            finally
            {
                lZipOutputStream.Finish();
                lZipOutputStream.Close();
            }
            return lZipOutputStream;
        }

        public static void UnZip(string pathArquivoZipado, string pathDirDestino)
        {
            if (!System.IO.File.Exists(pathArquivoZipado))
            {
                throw new System.IO.FileNotFoundException("O arquivo " + pathArquivoZipado + " não foi encontrado. UnZip cancelado.");
            }

            FileStream lFile = File.OpenRead(pathArquivoZipado);
            UnZip(lFile, pathDirDestino);

        }


        public static void UnZip(string pathArquivoZipado, string pathDirDestino, bool mesmaPasta)
        {
            if (!System.IO.File.Exists(pathArquivoZipado))
            {
                throw new System.IO.FileNotFoundException("O arquivo " + pathArquivoZipado + " não foi encontrado. UnZip cancelado.");
            }

            FileStream lFile = File.OpenRead(pathArquivoZipado);
            UnZip(lFile, pathDirDestino, mesmaPasta);

        }

        public static void UnZip(FileStream file, string pathDirDestino)
        {
            UnZip(file, pathDirDestino, true);
        }

        public static void UnZip(FileStream file, string pathDirDestino, bool mesmaPasta)
        {
            UnZip(file, pathDirDestino, true, mesmaPasta);
        }

        public static void UnZip(FileStream arquivo, bool fecharArquivo, string pathDirDestino)
        {
            using (ZipInputStream s = new ZipInputStream(arquivo))
            {
                ZipEntry theEntry;

                string diretorio = pathDirDestino;
                string nomeArquivo = String.Empty;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    string[] directories = theEntry.Name.Replace(':', '_').Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    diretorio = Path.Combine(pathDirDestino, string.Join(Path.DirectorySeparatorChar.ToString(), directories));
                    nomeArquivo = Path.GetFileName(diretorio);
                    diretorio = Path.GetDirectoryName(diretorio);
                    if (!Directory.Exists(diretorio))
                    {
                        Directory.CreateDirectory(diretorio);
                    }
                    if (theEntry.IsDirectory)
                    {
                        continue;
                    }
                    using (FileStream streamWriter = File.Open(Path.Combine(diretorio, Path.GetFileName(nomeArquivo)), FileMode.OpenOrCreate))
                    {

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Flush();
                    }
                }
            }

            if (fecharArquivo)
                arquivo.Close();
        }


        public static void UnZip(FileStream arquivo, string pathDirDestino, bool fecharArquivo, bool mesmaPasta)
        {
            using (ZipInputStream s = new ZipInputStream(arquivo))
            {
                ZipEntry theEntry;

                string diretorio = pathDirDestino;
                string nomeArquivo = String.Empty;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    string[] directories = theEntry.Name.Replace(':', '_').Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    string CaminhoValidoParaWindows = string.Join(Path.DirectorySeparatorChar.ToString(), directories);
                    if (!mesmaPasta)
                    {
                        diretorio = Path.Combine(pathDirDestino, CaminhoValidoParaWindows);
                    }
                    else
                    {
                        if (theEntry.IsDirectory)
                        {
                            continue;
                        }
                        diretorio = Path.Combine(pathDirDestino, Path.GetFileName(CaminhoValidoParaWindows));
                    }
                    nomeArquivo = Path.GetFileName(diretorio);

                    diretorio = Path.GetDirectoryName(diretorio);
                    if (!Directory.Exists(diretorio))
                    {
                        Directory.CreateDirectory(diretorio);
                    }

                    using (FileStream streamWriter = File.Open(Path.Combine(diretorio, Path.GetFileName(nomeArquivo)), FileMode.Create))
                    {

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Flush();
                    }
                }
            }

            if (fecharArquivo)
                arquivo.Close();
        }

        public static void UnZipDestino(string pathArquivoZipado, string pathDirDestino)
        {
            if (!System.IO.File.Exists(pathArquivoZipado))
            {
                throw new System.IO.FileNotFoundException("O arquivo " + pathArquivoZipado + " nao foi localizado. UnZip arquivo cancelada");
            }

            FileStream lFile = File.OpenRead(pathArquivoZipado);
            ZipInputStream s = new ZipInputStream(lFile);
            try
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    FileStream streamWriter = File.Create(pathDirDestino + @"\" + Path.GetFileName(theEntry.Name));

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    streamWriter.Close();
                }

            }
            finally
            {
                lFile.Close();
                s.Close();

            }

        }

    }
}
