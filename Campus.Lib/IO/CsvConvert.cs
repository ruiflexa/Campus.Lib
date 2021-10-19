using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campus.Lib.IO
{
    public class CsvConvert 
    {
        CsvReader _csrReader;
        CsvWriter _csvWrite;


        public CsvConvert()
        {
        }


        private void config<T>(CsvConfiguration csvConfig, Configuracao config)
        {
            config = config == null ? new Configuracao() : config;
            csvConfig.Delimiter = config.Delimitador;
            csvConfig.IgnoreHeaderWhiteSpace = config.IgnoreEspacoEmBrancoHeader;
            csvConfig.TrimFields = config.RemoverEspacoEmBrancoCampos;
            if (config.Map.Count > 0)
            {

                var customerMap = new CsvHelper.Configuration.DefaultCsvClassMap<T>();
                var sampleObject = Activator.CreateInstance<T>();
                foreach (var map in config.Map)
                {

                    var propertyInfo = sampleObject.GetType().GetProperty(map.NomeDaPropriedade);
                    var newMap = new CsvPropertyMap(propertyInfo);
                    newMap.Name(map.NomeDaColuna);
                    if (map.Converter != null)
                    {
                        newMap.ConvertUsing(
                            row => map.Converter.Conversao(row.GetField<string>(map.Converter.NomeDaColuna))
                            );
                    }
                    customerMap.PropertyMaps.Add(newMap);
                }
                csvConfig.RegisterClassMap(customerMap);
            }
        }

        public T ReadData<T>(string filePah, Configuracao configuracao = null)
        {
            using (StreamReader _sr = new StreamReader(filePah))
            {
                return ReadData<T>(_sr, configuracao);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="streamReader"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public T ReadData<T>(StreamReader streamReader, Configuracao configuracao = null)
        {
            _csrReader = new CsvReader(streamReader);
            config<T>(_csrReader.Configuration, configuracao);


            return _csrReader.GetRecord<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePah"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public T[] ReadAllData<T>(string filePah, Configuracao configuracao = null)
        {
            var encoding = configuracao == null ? Encoding.Default : configuracao.Encondig;
            using (StreamReader _sr = new StreamReader(filePah, encoding))
            {
                return ReadAllData<T>(_sr, configuracao);
            }
        }

        public T[] ReadAllData<T>(StreamReader streamReader, Configuracao configuracao = null)
        {
            _csrReader = new CsvReader(streamReader);
            config<T>(_csrReader.Configuration, configuracao);
            return _csrReader.GetRecords<T>().ToArray();
        }

        public void WriteData<T>(StreamWriter streamWriter, T[] dados, Configuracao configuracao = null)
        {
            _csvWrite = new CsvWriter(streamWriter);
            config<T>(_csvWrite.Configuration, configuracao);
            _csvWrite.WriteHeader<T>();
            foreach (T item in dados)
            {
                _csvWrite.WriteRecord<T>(item);
            }
        }


        public void WriteData<T>(string filePah, T[] dados, Configuracao configuracao = null)
        {
            var encoding = configuracao == null ? Encoding.Default : configuracao.Encondig;
            using (StreamWriter _sr = new StreamWriter(filePah, true, encoding))
            {
                WriteData(_sr, dados, configuracao);
                _sr.Close();
            }
        }
    }
}
