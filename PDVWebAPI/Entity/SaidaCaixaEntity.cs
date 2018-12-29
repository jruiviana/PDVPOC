using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity
{
   
    public interface ISaidaCaixa
    {
        Guid id { get; set; }
        decimal troco { get; set; }
        int qtdNotas100 { get; set; }
        int qtdNotas50 { get; set; }
        int qtdNotas20 { get; set; }
        int qtdNotas10 { get; set; }
        int qtdMoedas050 { get; set; }
        int qtdMoedas010 { get; set; }
        int qtdMoedas005 { get; set; }
        int qtdMoedas001 { get; set; }

        DateTime dataSaida { get; set; }
    }
    public class SaidaCaixa: ISaidaCaixa
    {
        public Guid id { get; set; }
        public decimal troco { get; set; }
        public int qtdNotas100 { get; set; }
        public int qtdNotas50 { get; set; }
        public int qtdNotas20 { get; set; }
        public int qtdNotas10 { get; set; }
        public int qtdMoedas050 { get; set; }
        public int qtdMoedas010 { get; set; }
        public int qtdMoedas005 { get; set; }
        public int qtdMoedas001 { get; set; }
        public DateTime dataSaida { get; set; }
    }
}
