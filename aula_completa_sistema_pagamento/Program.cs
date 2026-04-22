class Program
{
    static void Main()
    {
        IPagamento pagamento1 = new CartaoCredito("Gabriel");
        IPagamento pagamento2 = new Pix("Ana");

        ProcessadorPagamento processador1 = new ProcessadorPagamento(pagamento1);
        ProcessadorPagamento processador2 = new ProcessadorPagamento(pagamento2);

        processador1.Executar(150);
        processador2.Executar(75);
    }
}