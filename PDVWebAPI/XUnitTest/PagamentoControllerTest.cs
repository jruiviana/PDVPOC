using Application;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PDVWebAPI.Controllers;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTest
{
    public class PagamentoControllerTest
    {
        private PagamentoController pagamentoController;
        public PagamentoControllerTest()
        {
            IQueryable<SaidaCaixa> listSaidaCaixa = new List<SaidaCaixa>
            {
                new SaidaCaixa
                {
                    dataSaida=DateTime.Now,
                    qtdNotas100=0,
                    qtdNotas50=0,
                    qtdNotas20=0,
                    qtdNotas10=0,
                    qtdMoedas050=2,
                    qtdMoedas010=0,
                    qtdMoedas005=0,
                    qtdMoedas001=0,
                    troco=1.00m,
                    id=Guid.NewGuid()
                },
                new SaidaCaixa
                {
                    dataSaida=DateTime.Now,
                    qtdNotas100=5,
                    qtdNotas50=1,
                    qtdNotas20=1,
                    qtdNotas10=1,
                    qtdMoedas050=11,
                    qtdMoedas010=2,
                    qtdMoedas005=1,
                    qtdMoedas001=2,
                    troco=585.77m,
                    id=Guid.NewGuid()
                }

            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            var mockSet = new Mock<DbSet<SaidaCaixa>>();
            mockSet.As<IQueryable<SaidaCaixa>>().Setup(m => m.Provider).Returns(listSaidaCaixa.Provider);
            mockSet.As<IQueryable<SaidaCaixa>>().Setup(m => m.Expression).Returns(listSaidaCaixa.Expression);
            mockSet.As<IQueryable<SaidaCaixa>>().Setup(m => m.ElementType).Returns(listSaidaCaixa.ElementType);
            mockSet.As<IQueryable<SaidaCaixa>>().Setup(m => m.GetEnumerator()).Returns(listSaidaCaixa.GetEnumerator());

            var mockContext = new Mock<SQLContext>();
            mockContext.Setup(c => c.SaidaCaixa).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);


            SQLRepository repository = new SQLRepository(mockContext.Object);
            PagamentoService pagamentoService = new PagamentoService(repository);

            pagamentoController = new PagamentoController(pagamentoService, null);
            pagamentoController.ControllerContext.HttpContext = new DefaultHttpContext();
            pagamentoController.ControllerContext.HttpContext.Request.Headers["device-id"] = "20317";

        }
        [Fact]
        public void PagamentoControllerTest_Pagamento_Get_Sucesso()
        {


            //Act
            var actionResult = pagamentoController.Get();

            //Assert
            var viewResult = Assert.IsType<ActionResult<IEnumerable<ISaidaCaixa>>>(actionResult);
            Assert.IsAssignableFrom<IEnumerable<ISaidaCaixa>>(viewResult.Value);
            Assert.True(viewResult.Value.Count() == 2);

        }
        [Fact]
        public void PagamentoControllerTest_Pagamento_Post_Sucesso()
        {
            //Arrange
            
            Pagamento pagamento = new Pagamento() {
                valorDevido=200.00m,
                valorPago=200.00m
            };

            //Act
            var actionResult = pagamentoController.Post(pagamento);

            //Assert
            var viewResult = Assert.IsType<ActionResult<ISaidaCaixa>>(actionResult);
            Assert.IsAssignableFrom<SaidaCaixa>(viewResult.Value);
        }
        [Fact]
        public void PagamentoControllerTest_Pagamento_Post_Valor_Devido_Menor_Que_Zero()
        {
            //Arrange

            Pagamento pagamento = new Pagamento()
            {
                valorDevido = 0.00m,
                valorPago = 100.00m
            };

            //Act
            var actionResult = pagamentoController.Post(pagamento);

            //Assert
            var viewResult = Assert.IsType<ActionResult<ISaidaCaixa>>(actionResult);
            Assert.True(((ObjectResult)viewResult.Result).StatusCode==400);
            Assert.True(((ObjectResult)viewResult.Result).Value.Equals("Valor devido não pode ser menor ou igual a 0"));
        }
        [Fact]
        public void PagamentoControllerTest_Pagamento_Post_Valor_Pago_Menor_Valor_Devido()
        {
            //Arrange

            Pagamento pagamento = new Pagamento()
            {
                valorDevido = 200.00m,
                valorPago = 100.00m
            };

            //Act
            var actionResult = pagamentoController.Post(pagamento);

            //Assert
            var viewResult = Assert.IsType<ActionResult<ISaidaCaixa>>(actionResult);
            Assert.True(((ObjectResult)viewResult.Result).StatusCode == 400);
            Assert.True(((ObjectResult)viewResult.Result).Value.Equals("Valor pago menor que o devido faltam R$100,00"));
        }
        [Fact]
        public void PagamentoControllerTest_Pagamento_Post_Troco_Correto()
        {
            //Arrange

            Pagamento pagamento = new Pagamento()
            {
                valorDevido = 200.00m,
                valorPago = 300.00m
            };

            //Act
            var actionResult = pagamentoController.Post(pagamento);

            //Assert
            var viewResult = Assert.IsType<ActionResult<ISaidaCaixa>>(actionResult);
            Assert.IsAssignableFrom<SaidaCaixa>(viewResult.Value);
            Assert.True(viewResult.Value.troco== 100.00m);
        }
        [Fact]
        public void PagamentoControllerTest_Pagamento_Post_Numero_Notas_Moedas_Correto()
        {
            //Arrange

            Pagamento pagamento = new Pagamento()
            {
                valorDevido = 200.00m,
                valorPago = 350.58m
            };

            //Act
            var actionResult = pagamentoController.Post(pagamento);

            //Assert
            var viewResult = Assert.IsType<ActionResult<ISaidaCaixa>>(actionResult);
            Assert.IsAssignableFrom<SaidaCaixa>(viewResult.Value);
            Assert.True(viewResult.Value.troco == 150.58m);
            Assert.True(viewResult.Value.qtdNotas100 == 1);
            Assert.True(viewResult.Value.qtdNotas50 == 1);
            Assert.True(viewResult.Value.qtdNotas20 == 0);
            Assert.True(viewResult.Value.qtdNotas10 == 0);
            Assert.True(viewResult.Value.qtdMoedas050 == 1);
            Assert.True(viewResult.Value.qtdMoedas010 == 0);
            Assert.True(viewResult.Value.qtdMoedas005 == 1);
            Assert.True(viewResult.Value.qtdMoedas001 == 3);
        }
        [Fact]
        public void PagamentoControllerTest_Pagamento_Post_Excecao_nao_esperada()
        {
            //Arrange

            Pagamento pagamento = null;

            //Act
            var actionResult = pagamentoController.Post(pagamento);

            //Assert
            var viewResult = Assert.IsType<ActionResult<ISaidaCaixa>>(actionResult);
            Assert.True(((ObjectResult)viewResult.Result).StatusCode == 500);
            Assert.Contains("Ocorreu um erro interno contate o administrador :",((ObjectResult)viewResult.Result).Value.ToString());

        }
        
    }
}
