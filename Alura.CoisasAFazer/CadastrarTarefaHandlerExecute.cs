using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using System;
using Xunit;
using System.Linq;

namespace Alura.CoisasAFazer
{
    public class CadastrarTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveIncluirNoBancoDeDados()
        {
            //arranje
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var repo = new RepositorioFake();

            var handler = new CadastraTarefaHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            var tarefa = repo.ObtemTarefas(t => t.Titulo.Equals("Estudar Xunit")).FirstOrDefault();
            Assert.NotNull(tarefa);

            // criar comando
            //executar o comando

        }
    }
}
