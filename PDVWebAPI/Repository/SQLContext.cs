using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace Repository
{
   
    public class SQLContext: DbContext
    {
        public SQLContext(DbContextOptions<SQLContext> options) : base(options)
        {
            
        }
        public SQLContext()
        {

        }
        public virtual DbSet<SaidaCaixa> SaidaCaixa { get; set; }
    }
}
