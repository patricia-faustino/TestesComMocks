﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Moq;
using Xunit;

namespace Alura.CoisasAFazer
{
    public class ObtemCategoriaPorIdExecute
    {
        [Fact]
        public void QuandoIdExistenteDeveChamarCategoriaPorIdUmaUnicaVez()
        {
            // arrange
            var idCategoria = 20;

            var comando = new ObtemCategoriaPorId(idCategoria);

            var mock = new Mock<IRepositorioTarefas>();

            var repo = mock.Object;

            var handler = new ObtemCategoriaPorIdHandler(repo);
            //act
            handler.Execute(comando);

            //assert
            mock.Verify(r => r.ObtemCategoriaPorId(idCategoria), Times.Once());
        }
    }
}
