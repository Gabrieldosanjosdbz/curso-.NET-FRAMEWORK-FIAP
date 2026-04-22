class Program
{
    static void Main()
    {
        // List
        List<string> usuarios = new List<string>
        {
            "Gabriel",
            "Ana",
            "Carlos",
            "Ana" // duplicado permitido
        };

        // HashSet (remover duplicados)
        HashSet<string> usuariosUnicos = new HashSet<string>(usuarios);

        // Dictionary (mapear usuário -> idade)
        Dictionary<string, int> idades = new Dictionary<string, int>
        {
            { "Gabriel", 25 },
            { "Ana", 17 },
            { "Carlos", 30 }
        };

        // Queue (fila de atendimento)
        Queue<string> fila = new Queue<string>(usuariosUnicos);

        // Stack (histórico de atendimentos)
        Stack<string> historico = new Stack<string>();

        Console.WriteLine("=== Atendimento ===");

        while (fila.Count > 0)
        {
            var usuario = fila.Dequeue();
            Console.WriteLine($"Atendendo: {usuario}");

            historico.Push(usuario);
        }

        Console.WriteLine("\n=== Histórico (último atendido primeiro) ===");

        foreach (var item in historico)
        {
            Console.WriteLine(item);
        }

        // LINQ: filtrar maiores de idade
        var maiores = idades
            .Where(x => x.Value >= 18)
            .Select(x => x.Key);

        Console.WriteLine("\n=== Maiores de idade ===");

        foreach (var nome in maiores)
        {
            Console.WriteLine(nome);
        }

        // Verificação com HashSet
        Console.WriteLine("\nExiste Gabriel?");
        Console.WriteLine(usuariosUnicos.Contains("Gabriel"));
    }
}