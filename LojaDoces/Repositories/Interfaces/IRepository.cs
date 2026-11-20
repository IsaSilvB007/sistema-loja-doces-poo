namespace LojaDoces.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    // 1. Guardar uma entidade nova (não devolve nada)
    void Adicionar(T entidade);

    // 2. Atualizar uma entidade que já existe (não devolve nada)
    void Atualizar(T entidade);

    // 3. Remover pelo id (não devolve nada)
    void Remover(int id);

    // 4. Buscar uma entidade pelo id (pode não existir!)
    T? ObterPorId(int id);

    // 5. Listar todas as entidades
    IEnumerable<T> ObterTodos();

}