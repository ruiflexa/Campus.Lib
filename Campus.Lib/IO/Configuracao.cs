using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campus.Lib.IO
{
    public class Configuracao
    {
        
       
        
        public Configuracao()
        {
            Delimitador = ";";
            IgnoreEspacoEmBrancoHeader = true;
            Map = new List<Mapeamento>();
            Encondig = Encoding.Default;
        }


        
       
        
        public string Delimitador { get; set; }

        public bool IgnoreEspacoEmBrancoHeader { get; set; }
        
        
        public bool RemoverEspacoEmBrancoCampos { get; set; }

        
        public Encoding Encondig { get; set; }

        
        public ICollection<Mapeamento> Map;

    }


    
 
    
    public class Mapeamento
    {
        

        
        private Mapeamento()
        {
        }


        public Mapeamento(string nomeDaPropriedade, string nomeDaColuna) : this()
        {
            NomeDaPropriedade = nomeDaPropriedade;
            NomeDaColuna = nomeDaColuna;
        }

        public Mapeamento(string nomeDaPropriedade) : this(nomeDaPropriedade, nomeDaPropriedade)
        {
        }


        
        public string NomeDaPropriedade { get; set; }
        

        
        public string NomeDaColuna { get; set; }
        
  
        public TipoConversao Converter { get; private set; }

        public void AddConvert(Func<string, dynamic> conversao)
        {
            AddConvert(NomeDaColuna, conversao);
        }


        public void AddConvert(string nomeDaColunae, Func<string, dynamic> conversao)
        {
            Converter = new TipoConversao()
            {
                NomeDaColuna = nomeDaColunae,
                Conversao = conversao
            };
        }
    }


    
    public class TipoConversao
    {

        
        public string NomeDaColuna { get; set; }

        public Func<string, dynamic> Conversao { get; set; }
    }
}
