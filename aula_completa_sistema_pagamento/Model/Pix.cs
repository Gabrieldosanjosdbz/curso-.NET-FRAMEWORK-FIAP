class Pix : PagamentoBase
{
    public Pix(string nome) : base(nome) {}

    public override void Pagar(double valor)
    {
        Console.WriteLine($"Pagamento de R$ {valor} via PIX.");
    }
}