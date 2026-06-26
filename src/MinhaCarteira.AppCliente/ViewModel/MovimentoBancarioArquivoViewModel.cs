using Dhani.Utilitarios.ViewModel;
using MinhaCarteira.AppCliente.Models.Interface;
using System;

namespace MinhaCarteira.AppCliente.ViewModel
{
    public class MovimentoBancarioArquivoViewModel : BaseViewModel, IEntidade<Guid>
    {
        public Guid Id { get; set; }
        public bool Deletado { get; set; }
        public DateTime DataCadastro { get; set; }

        public Guid ArquivoId { get; set; }
        public ArquivoViewModel Arquivo { get; set; }

        public Guid MovimentoBancarioId { get; set; }
        public MovimentoBancarioViewModel MovimentoBancario { get; set; }
    }
}
