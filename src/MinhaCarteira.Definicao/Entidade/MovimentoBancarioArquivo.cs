using MinhaCarteira.Definicao.Interface.Entidade;
using System;

namespace MinhaCarteira.Definicao.Entidade
{
    public class MovimentoBancarioArquivo : IEntidade<Guid>
    {
        public Guid Id { get; set; }
        public bool Deletado { get; set; }
        public DateTime DataCadastro { get; set; }

        public Guid? ArquivoId { get; set; }
        public Arquivo Arquivo { get; set; }

        public Guid? MovimentoBancarioId { get; set; }
        public MovimentoBancario MovimentoBancario { get; set; }
    }
}
