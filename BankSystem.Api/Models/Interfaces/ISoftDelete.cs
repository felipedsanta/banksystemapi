namespace BankSystem.Api.Models.Interfaces
{
    public interface ISoftDelete
    {
        bool Ativo { get; }
        DateTime? DataAtualizacao { get; }
        void Excluir();
        void Restaurar();
    }
}
