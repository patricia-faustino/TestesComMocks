using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using System;
using Xunit;
using System.Linq;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.EntityFrameworkCore;

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

    }
}
