class Program
{
    static void Main(String[] args)
    {
        // Tratando erro em dotnet 
        try
        {
            throw new PersonalizadaExceptionException("Mensagem de erro");
        }
        catch (PersonalizadaExceptionException e)
        {
            Console.WriteLine("Achou o erro: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Erro");
        }
        finally
        {
            Console.WriteLine("Bobou");
        }
        Console.Read();
    }
}