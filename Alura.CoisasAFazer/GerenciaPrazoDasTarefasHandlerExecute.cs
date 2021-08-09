using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {

        [Fact]
        public void QuandoTarefasEstiveremAtrasadasDeveMudarSeuStatus()
        {
            //arrange

            var compCateg = new Categoria(1, "Compras");
            var casaCateg = new Categoria(2, "Casa");
            var trabCateg = new Categoria(3, "Trabalho");
            var saudCateg = new Categoria(4, "Saúde");
            var higiCateg = new Categoria(5, "Higiente");

            var tarefas = new List<Tarefa>
            {
                //atrasadas
                new Tarefa(1, "Tirar lixo", casaCateg, new DateTime(2018, 12, 31), null, StatusTarefa.Criada),
                new Tarefa(4, "Fazer almoço", casaCateg, new DateTime(2017, 12, 31), null, StatusTarefa.Criada),
                new Tarefa(9, "Ir á academia", saudCateg, new DateTime(2018, 12, 31), null, StatusTarefa.Criada),
                new Tarefa(7, "Concluir relatório", trabCateg, new DateTime(2018, 5, 27), null, StatusTarefa.Pendente),
                new Tarefa(10, "Beber água", saudCateg, new DateTime(2018, 12, 31), null, StatusTarefa.Criada),

                //dentro do prazo 

                new Tarefa(8, "Arrumar a cama", casaCateg, new DateTime(2021, 1, 31), null, StatusTarefa.Criada),
                new Tarefa(2, "Escovar os dentes", higiCateg, new DateTime(2021, 8, 8), null, StatusTarefa.Criada),
                new Tarefa(3, "Comprar ração", compCateg, new DateTime(2021, 12, 31), null, StatusTarefa.Criada),
                new Tarefa(5, "Comprar presente para o João", compCateg, new DateTime(2021, 10, 31), null, StatusTarefa.Criada),
            };


            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(options);

            var repo = new RepositorioTarefa(contexto);

            repo.IncluirTarefas(tarefas.ToArray());

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2021,1,1));

            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);

            //asert
            var tarefasEmAtraso = repo.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(5, tarefasEmAtraso.Count());

        }

        [Fact]
        public void QuandoInvocadoDeveCXhamarAtualizarTarefasNaQtdeVezesDoTotalDeTarefasAtrasadas()
        {
            var categoria = new Categoria("Dummy");

            var tarefas = new List<Tarefa>
            {
                //atrasadas
                new Tarefa(1, "Tirar lixo", categoria, new DateTime(2018, 12, 31), null, StatusTarefa.Criada),
                new Tarefa(4, "Fazer almoço", categoria, new DateTime(2017, 12, 31), null, StatusTarefa.Criada),
                new Tarefa(9, "Ir á academia", categoria, new DateTime(2018, 12, 31), null, StatusTarefa.Criada),
            };

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>()))
                .Returns(tarefas);

            var repo = mock.Object;
            var comando = new GerenciaPrazoDasTarefas(new DateTime(2021, 1, 1));

            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());
        }


    }
}
