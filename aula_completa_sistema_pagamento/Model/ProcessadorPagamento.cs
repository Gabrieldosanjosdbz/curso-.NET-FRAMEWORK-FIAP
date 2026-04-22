class ProcessadorPagamento
{
    private IPagamento _pagamento;

    public ProcessadorPagamento(IPagamento pagamento)
    {
        _pagamento = pagamento;
    }

    public void Executar(double valor)
    {
        _pagamento.Pagar(valor);
    }
}
