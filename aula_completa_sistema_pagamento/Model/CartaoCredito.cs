class CartaoCredito : PagamentoBase
{
    public CartaoCredito(string nome) : base(nome) {}

    public override void Pagar(double valor)
    {
        Console.WriteLine($"Pagamento de R$ {valor} no cartão de crédito.");
    }
}