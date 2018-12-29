using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity
{
    public interface IPagamento
    {
        decimal valorDevido { get; set; }
        decimal valorPago { get; set; }
    }
    public class Pagamento: IPagamento
    {
        public decimal valorDevido { get; set; }
        public decimal valorPago { get; set; }
    }
    
    
}
