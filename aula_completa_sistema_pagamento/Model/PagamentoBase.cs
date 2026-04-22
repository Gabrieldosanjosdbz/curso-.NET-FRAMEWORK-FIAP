abstract class PagamentoBase : IPagamento
{
    public string NomeCliente { get; set; }

    public PagamentoBase(string nome)
    {
        NomeCliente = nome;
    }

    public abstract void Pagar(double valor);

    public void ExibirCliente()
    {
        Console.WriteLine($"Cliente: {NomeCliente}");
    }
}