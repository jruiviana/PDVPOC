using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Entity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;


namespace PDVWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class PagamentoController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly ILogger _logger;

        public PagamentoController(IPagamentoService pagamentoService,ILoggerFactory DepLoggerFactory)
        {
            _pagamentoService = pagamentoService;
            if (DepLoggerFactory != null) _logger = DepLoggerFactory.CreateLogger("Controllers.PagamentoController");
        }
        // GET api/values
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<ISaidaCaixa>> Get()
        {
            try
            {
                return _pagamentoService.ConsultaSaidas().ToList();
            }
            catch (Exception ex)
            {

                if (_logger != null) _logger.LogError(ex, ex.Message);
                return StatusCode(500, "Ocorreu um erro interno contate o administrador :" + ex.Message);
            }
            
        }

       
        // POST api/values
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<ISaidaCaixa> Post([FromBody]Pagamento value)
        {
            try
            {
               

                //retornando informação para o cliente
                return _pagamentoService.ExecutarPagamento(value);

            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                if (_logger != null) _logger.LogError(ex, ex.Message);
                return StatusCode(500, "Ocorreu um erro interno contate o administrador :" + ex.Message);
            }

        }

       
    }
}
