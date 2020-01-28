using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campus.Lib.IO
{
    public static class ExcelHelper
    {
        private static string _connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;{1}{2}""";
        private static string _definiCamposComoTexto = "IMEX=1;";
        private static string _considerarPrimeiraLinhaComoCabecalho = "HDR=Yes;";

        private static string ConfigurarStringConexao(string arquivo, bool considerarValoresComoTexto, bool considerarPrimeiraLinhaComoCabecalho)
        {
            if (!considerarPrimeiraLinhaComoCabecalho)
                _considerarPrimeiraLinhaComoCabecalho = "HDR=NO;";
            string conexao = string.Format(_connStr, arquivo, considerarValoresComoTexto ? _considerarPrimeiraLinhaComoCabecalho : "",
                considerarValoresComoTexto ? _definiCamposComoTexto : "");
            return conexao;
        }

        private static List<string> ObterNomePlanilhas(DbConnection connection)
        {
            List<string> planilhas = new List<string>();
            DataTable dt = connection.GetSchema("Tables");
            foreach (DataRow dr in dt.Rows)
                planilhas.Add(dr["TABLE_NAME"].ToString().ToLower());
            return planilhas;
        }

        public static DataSet ObterPlanilhas(string arquivo, bool considerarValoresComoTexto, bool considerarPrimeiraLinhaComoCabecario)
        {
            var str = ConfigurarStringConexao(arquivo, considerarValoresComoTexto, considerarPrimeiraLinhaComoCabecario);
            var dataSet = new DataSet();
            using (OleDbConnection conn = new OleDbConnection(str))
            {
                conn.Open();
                var nomeDasPlanilhas = ObterNomePlanilhas(conn);
                foreach (var planilha in nomeDasPlanilhas)
                {
                    var sql = string.Format("select * from [{0}]", planilha);
                    var adapter = new OleDbDataAdapter(sql, conn);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataSet.Tables.Add(dataTable);
                }
            }
            return dataSet;
        }

        public static DataSet ObterPlanilhasComCabecalho(string arquivo, bool considerarValoresComoTexto, bool considerarPrimeiraLinhaComoCabecalho)
        {
            var str = ConfigurarStringConexao(arquivo, considerarValoresComoTexto, considerarPrimeiraLinhaComoCabecalho);
            var dataSet = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(str))
            {
                conn.Open();
                var nomeDasPlanilhas = ObterNomePlanilhas(conn);
                foreach (var planilha in nomeDasPlanilhas)
                {
                    var sql = string.Format("select * from [{0}]", planilha);
                    var adapter = new OleDbDataAdapter(sql, conn);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Remove the first row (header row)
                    DataRow rowDel = dataTable.Rows[0];
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                        dataTable.Columns[i].ColumnName = rowDel.ItemArray[i].ToString();

                    dataTable.Rows.Remove(rowDel);

                    dataSet.Tables.Add(dataTable);
                }

            }
            return dataSet;
        }

    }
}
