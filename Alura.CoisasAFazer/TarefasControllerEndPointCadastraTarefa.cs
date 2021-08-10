using Xunit;
using Moq;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Core.Models;

namespace Alura.CoisasAFazer
{
    public class TarefasControllerEndPointCadastraTarefa
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveRetornar200()
        {

            //arranje
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(options);

            contexto.Categorias.Add(new Categoria(20, "Estudo"));

            contexto.SaveChanges();

            var repo = new RepositorioTarefa(contexto);

            var controlador = new TarefasController(repo, mockLogger.Object);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar XUnit";
            model.Prazo = new DateTime(2021, 12, 31);

            //act 
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<OkResult>(retorno); //200
        }

        [Fact]
        public void QuandoExcecaoForLancadaDeveRetornarStatusCode500() {
            //arranje
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudo"));

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));

            var repo = mock.Object;

            var controlador = new TarefasController(repo, mockLogger.Object);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar XUnit";
            model.Prazo = new DateTime(2021, 12, 31);

            //act 
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<StatusCodeResult>(retorno);
            var statusCodeRetornado = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCodeRetornado);
        }
    }
}
