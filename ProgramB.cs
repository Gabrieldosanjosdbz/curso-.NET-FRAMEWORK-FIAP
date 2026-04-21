using System;
using Microsoft.VisualBasic;

class ProgramB
{
    public static void Main(string[] args)
    {
        // I/O
        Console.Write("Digite seu nome: ");
        string nome = Console.ReadLine();

        Console.Write("Digite sua idade: ");
        int idade;
        while (!int.TryParse(Console.ReadLine(), out idade))
        {
            Console.Write("Valor inválido. Digite um número: ");
        }

        // Condicional
        if (idade >= 18)
        {
            Console.WriteLine($"{nome} é maior de idade.");
        }
        else
        {
            Console.WriteLine($"{nome} é menor de idade.");
        }

        // Array
        int[] numeros = new int[5];

        Console.WriteLine("Digite 5 números:");

        // Loop for
        for (int i = 0; i < numeros.Length; i++)
        {
            Console.Write($"Número {i + 1}: ");
            numeros[i] = int.Parse(Console.ReadLine());
        }

        // Processamento
        int soma = 0;

        // foreach
        foreach (int n in numeros)
        {
            soma += n;
        }

        double media = (double)soma / numeros.Length;

        Console.WriteLine($"Soma: {soma}");
        Console.WriteLine($"Média: {media}");

        // switch
        Console.Write("Digite um número de 1 a 3: ");
        int opcao = int.Parse(Console.ReadLine());

        switch (opcao)
        {
            case 1:
                Console.WriteLine("Opção 1 escolhida");
                break;
            case 2:
                Console.WriteLine("Opção 2 escolhida");
                break;
            case 3:
                Console.WriteLine("Opção 3 escolhida");
                break;
            default:
                Console.WriteLine("Opção inválida");
                break;
        }

        // while
        int contador = 0;
        Console.WriteLine("Contagem até 3:");

        while (contador < 3)
        {
            Console.WriteLine(contador);
            contador++;
        }

        // do-while
        int x = 0;
        do
        {
            Console.WriteLine($"Executou ao menos uma vez: {x}");
            x++;
        } while (x < 1);

        // Chamando função estatica de outra classe
        ProgramA.aula();

        // Chamando função local 
        funcaoLocal(1); 
    }

    private static void funcaoLocal(int varivael)
    {
        Console.WriteLine("Isto é uma função local e esse é sua variavel: " + varivael);
    }
}