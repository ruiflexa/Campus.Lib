using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campus.Lib.IO
{
    public static class CsvHelper
    {
        public static DataTable ObterDataTable(string caminhoArquivo)
        {
            return ObterDataTable(caminhoArquivo, ",", true);
        }

        public static DataTable ObterDataTable(string caminhoArquivo, string separador, bool possuiCabecalho)
        {
            using (StreamReader stream = new StreamReader(caminhoArquivo, Encoding.Default))
            {
                return ObterDataTable(stream.BaseStream, separador, possuiCabecalho);
            }
        }

        public static DataTable ObterDataTable(Stream stream)
        {
            return ObterDataTable(stream, ",", true);
        }

        public static DataTable ObterDataTable(Stream stream, string separador, bool possuiCabecalho)
        {
            StreamReader arquivo = new StreamReader(stream);
            bool ehLinhaCabecalho = possuiCabecalho;
            DataTable retornoDT = new DataTable();
            while (!arquivo.EndOfStream)
            {
                string[] colunas = arquivo.ReadLine().Split(new string[] { separador }, StringSplitOptions.None);
                if (ehLinhaCabecalho)
                {
                    foreach (string coluna in colunas)
                    {
                        retornoDT.Columns.Add(coluna);
                    }
                    
                    ehLinhaCabecalho = false;
                }
                else
                {
                    while (retornoDT.Columns.Count < colunas.Length)
                    {
                        retornoDT.Columns.Add();
                    }

                    DataRow row = retornoDT.NewRow();
                    for (int i = 0; i < colunas.Length; i++)
                    {
                        row[i] = colunas[i];
                    }
                    retornoDT.Rows.Add(row);
                }
            }
            return retornoDT;
        }

        public static void GravarStream(IDataReader reader, Stream stream)
        {
            GravarStream(reader, stream, true);
        }

        public static void GravarStream(IDataReader reader, Stream stream, bool incluirCabecalho)
        {
            StreamWriter arquivo = new StreamWriter(stream);
            try
            {

                bool primeiraColuna = true;
                if (incluirCabecalho)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        arquivo.Write("{0}\"{1}\"", (primeiraColuna ? "" : ";"), reader.GetName(i));
                        primeiraColuna = false;
                    }
                    arquivo.WriteLine();
                }
                while (reader.Read())
                {
                    primeiraColuna = true;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object valor = reader[i];
                        if ((valor == null) || (valor == DBNull.Value))
                        {
                            valor = String.Empty;
                        }
                        else if (valor.GetType() == typeof(string))
                        {
                            valor = valor.ToString().Replace("\"", "\"\"");
                        }
                        arquivo.Write("{0}\"{1}\"", (primeiraColuna ? "" : ";"), valor);
                        primeiraColuna = false;
                    }
                    arquivo.WriteLine();
                }
            }
            finally
            {
                arquivo.Flush();
            }
        }

        public static Stream ObterStreamCSV(DataTable dataTable)
        {
            MemoryStream memory = new MemoryStream();
            StreamWriter arquivo = new StreamWriter(memory);
            try
            {
                bool primeiraColuna = true;
                foreach (DataColumn column in dataTable.Columns)
                {
                    string cabecalhoColuna = String.IsNullOrEmpty(column.Caption) ? column.ColumnName : column.Caption;
                    arquivo.Write("{0}\"{1}\"", (primeiraColuna ? "" : ";"), cabecalhoColuna);
                    primeiraColuna = false;
                }
                arquivo.WriteLine();

                foreach (DataRow row in dataTable.Rows)
                {
                    primeiraColuna = true;
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        object valor = row[column];
                        if ((valor == null) || (valor == DBNull.Value))
                        {
                            valor = String.Empty;
                        }
                        else if (valor.GetType() == typeof(string))
                        {
                            valor = valor.ToString().Replace("\"", "\"\"");
                        }
                        arquivo.Write("{0}\"{1}\"", (primeiraColuna ? "" : ";"), valor);
                        primeiraColuna = false;
                    }
                    arquivo.WriteLine();
                }
                arquivo.Flush();
            }
            catch
            {
                arquivo.Close();
                memory.Close();
                throw;
            }
            return memory;
        }
    }
}
