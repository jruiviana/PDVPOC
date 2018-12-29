using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public interface IRepository
    {
        IEnumerable<SaidaCaixa> GetAllSaidaCaixa();
        ISaidaCaixa SaveSaidaCaixa(ISaidaCaixa saidaCaixa);

    }
    public class SQLRepository : IRepository
    {
        private readonly SQLContext _context;

        public SQLRepository(SQLContext context)
        {
            _context = context;
            if(_context.Database != null) _context.Database.EnsureCreated();
        }
        public IEnumerable<SaidaCaixa> GetAllSaidaCaixa()
        {
            return _context.SaidaCaixa.Where(x => x.troco > 0);
        }

        public ISaidaCaixa SaveSaidaCaixa(ISaidaCaixa saidaCaixa)
        {
            _context.SaidaCaixa.Add((SaidaCaixa)saidaCaixa);
            _context.SaveChanges();

            return saidaCaixa;
        }
    }
}
