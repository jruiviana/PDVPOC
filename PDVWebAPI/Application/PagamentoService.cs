using Entity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application
{
    public interface IPagamentoService
    {
        SaidaCaixa ExecutarPagamento(IPagamento pagamento);
        IEnumerable<SaidaCaixa> ConsultaSaidas();
    }
    public class PagamentoService : IPagamentoService
    {
        private readonly IRepository _repository;

        public PagamentoService(IRepository repository)
        {
            _repository = repository;           
        }

        public IEnumerable<SaidaCaixa> ConsultaSaidas()
        {
            
            return _repository.GetAllSaidaCaixa();
        }

        public SaidaCaixa ExecutarPagamento(IPagamento pagamento)
        {
           
                SaidaCaixa saidaCaixa = new SaidaCaixa();
            //calculando o valor total do troco;
            saidaCaixa.troco = pagamento.valorPago - pagamento.valorDevido;

                //verificando se o valor devido não é menor ou igual a 0
                if (pagamento.valorDevido <= 0)
                    throw new ApplicationException("Valor devido não pode ser menor ou igual a 0");

                //verificando se o valor pago não é menor que o valor devido
                if (saidaCaixa.troco < 0)
                    throw new ApplicationException(string.Format("Valor pago menor que o devido faltam R${0}", (saidaCaixa.troco * -1).ToString()));


            decimal trocoDisponivel = saidaCaixa.troco;
                //Calculo das notas
                int[] NotasDisponiveis = { 100, 50, 20, 10 };
                int[] qtdNotas = { 0, 0, 0, 0 };

                for (int i = 0; i < NotasDisponiveis.Length; i++)
                {
                    if (trocoDisponivel < 10)
                        break;
                    qtdNotas[i] = Convert.ToInt32(Math.Truncate(trocoDisponivel / NotasDisponiveis[i]));
                    trocoDisponivel = trocoDisponivel % NotasDisponiveis[i];
                }

                //Calculo moedas
                decimal[] MoedasDisponiveis = { 0.50m, 0.10m, 0.05m, 0.01m };
                int[] qtdMoedas = { 0, 0, 0, 0 };

                for (int i = 0; i < MoedasDisponiveis.Length; i++)
                {
                    if (trocoDisponivel < 0.01m)
                        break;
                    qtdMoedas[i] = Convert.ToInt32(Math.Truncate(trocoDisponivel / MoedasDisponiveis[i]));
                    trocoDisponivel = trocoDisponivel % MoedasDisponiveis[i];
                }

            //preenchendo objeto de retorno
            saidaCaixa.qtdNotas100 = qtdNotas[0];
            saidaCaixa.qtdNotas50 = qtdNotas[1];
            saidaCaixa.qtdNotas20 = qtdNotas[2];
            saidaCaixa.qtdNotas10 = qtdNotas[3];

            saidaCaixa.qtdMoedas050 = qtdMoedas[0];
            saidaCaixa.qtdMoedas010 = qtdMoedas[1];
            saidaCaixa.qtdMoedas005 = qtdMoedas[2];
            saidaCaixa.qtdMoedas001 = qtdMoedas[3];
            saidaCaixa.dataSaida = DateTime.Now;

            //salvando troco saida de caixa no banco de dados

            return (SaidaCaixa)_repository.SaveSaidaCaixa(saidaCaixa);
             

            
            
           
        }
    }
}
