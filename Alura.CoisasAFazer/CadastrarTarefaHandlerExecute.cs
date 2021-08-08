using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using System;
using Xunit;
using System.Linq;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Alura.CoisasAFazer
{
    public class CadastrarTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveIncluirNoBancoDeDados()
        {
            //arranje
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(options);

            var repo = new RepositorioTarefa(contexto);

            var handler = new CadastraTarefaHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            var tarefa = repo.ObtemTarefas(t => t.Titulo.Equals("Estudar Xunit")).FirstOrDefault();
            Assert.NotNull(tarefa);

            // criar comando
            //executar o comando

        }


        [Fact]
        public void QuandoExceptionForLancadaRestultadoIsSucessDeveSerFalse()
        {
            //arranje
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert

            Assert.False(resultado.IsSucess);
        }
    }
}
