namespace MinhaCarteira.AppCliente.Models.Interface;

public interface IEntidade<TPK>
{
    TPK Id { get; set; }
    bool Deletado { get; set; }
}