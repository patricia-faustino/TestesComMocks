using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace Alura.CoisasAFazer
{
    public class CadastrarTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveIncluirNoBancoDeDados()
        {
            //arranje
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var mock = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(options);

            var repo = new RepositorioTarefa(contexto);

            var handler = new CadastraTarefaHandler(repo, mock.Object);

            //act
            handler.Execute(comando);

            //assert
            var tarefa = repo.ObtemTarefas(t => t.Titulo.Equals("Estudar Xunit")).FirstOrDefault();
            Assert.NotNull(tarefa);

        }


        [Fact]
        public void QuandoExceptionForLancadaRestultadoIsSucessDeveSerFalse()
        {
            //arranje
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert

            Assert.False(resultado.IsSucess);
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveLogarAMensagemDaExcecao()
        {

            //arrange
            var mensagemDeErroEsperada = "Houve um erro na inclusão de tarefas";
            var excecaoEsperada = new Exception(mensagemDeErroEsperada);

            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(excecaoEsperada);

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            mockLogger.Verify(l =>
                           l.Log(
                               LogLevel.Error,      // nivel de log
                               It.IsAny<EventId>(), // identificador do evento 
                               It.Is<It.IsAnyType>((v, t) => true),  // objeto que será logado
                               excecaoEsperada,    // exceção que sera logada
                               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), // função que converte objeto + exceção em string
                           Times.Once());
        }

        delegate void CapturaMensagemLog(LogLevel level, EventId eventId, object state, Exception exception, Func<object, Exception, string> function);

        [Fact]
        public void DadaTarefaComInfoValidasDeveLogar()
        {
            //arrange
            var tituloTarefaEsperado = "Usar Moq para aprofundar conhecimento de API";
            var comando = new CadastraTarefa(tituloTarefaEsperado, new Categoria(100, "Estudo"), new DateTime(2019, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            LogLevel levelCapturado = LogLevel.Debug;
            string mensagemCapturada = string.Empty;

            CapturaMensagemLog captura = (level, eventId, state, exception, func) =>
            {
                levelCapturado = level;
                mensagemCapturada = func(state, exception);
            };

            mockLogger.Setup(l =>
                l.Log(
                    It.IsAny<LogLevel>(), //nível de log => LogError
                    It.IsAny<EventId>(), //identificador do evento
                    It.IsAny<object>(), //objeto que será logado
                    It.IsAny<Exception>(),    //exceção que será logada
                    It.IsAny<Func<object, Exception, string>>() //função que converte objeto+exceção >> string)
                )).Callback(captura);

            var mock = new Mock<IRepositorioTarefas>();

            var handler = new CadastraTarefaHandler(mock.Object, mockLogger.Object);

            //act
            handler.Execute(comando); //SUT >> CadastraTarefaHandlerExecute

            //assert
            Assert.Equal(LogLevel.Debug, levelCapturado);
            Assert.Contains(tituloTarefaEsperado, mensagemCapturada);
        }
    }
}
