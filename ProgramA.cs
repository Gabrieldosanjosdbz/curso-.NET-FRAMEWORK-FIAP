using System;

class ProgramA
{
    public static void aula()
    {
        //int valor; sem valor
        int valor = 10;
        valor += 1;
        valor *= 2;

        Console.WriteLine(valor);

        if (valor == 22) // não existe === aqui
        {
            Console.WriteLine("É 22");
        } else
        {
            Console.WriteLine("Não é 22");
        }

        // O var assume a tipagem do primeiro valor atribuido a variavel
        var inteiro = 1;
        inteiro = 2;

        var palavra = "pão";
        palavra = "roupa";
         /*
            double valorDouble = 10.5;

            bool validacao = true;

            // As duas funciona da mesma forma, o minusculo é apenas um alias para a String 
            string nome = "fiap";
            String nome2 = "Gabriel";
         */

        // Foreach e array no c#
        String[] pessoas = {"Gabriel", "Lucas", "Rubia", "Alan"};
        //String[] numeros = new String[3]; declarando array com tamanho definido 
        
        // pode usar var também
        foreach (String pessoa in pessoas)
        {
            Console.WriteLine("Eu amo o/a: " + pessoa);
        }
    }
}