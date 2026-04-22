using System.Net.Http.Headers;

class Program
{
    static void Main(String[] args)
    {
        // Declarando um array simples
        String[] valores = {"a", "b", "c", "d"};
        // Declarando um array sem valores, mas demilitando seu tamanho 
        String[] valores2 = new String[10];
        // Matriz 
        String[,] valores3 = new String[5,3];

        // Colletions (muito parecido com java aqui)
        IList<String> lista = new List<String>();

        // Push (pilha) 
        lista.Add("Gabriel");
        lista.Add("Lucas");

        // Escolhendo posição para adicionar elemento (sem apagar)
        lista.Insert(1, "Corinthains");
        // Escolhendo posição que quero remover
        lista.RemoveAt(2);

        foreach (var item in lista)
        {
            System.Console.WriteLine(item);
        }

        // Dicionario (json). Chave e valor em DOTNET
        IDictionary<int, String> json = new Dictionary<int, String>();
        json[1] = "Bom";
        json[2] = "Dia";

        foreach (KeyValuePair<int, String> item in json)
        {
            //pegando a chave
            System.Console.WriteLine(item.Key);
            
            //Pegando valor
            System.Console.WriteLine(item.Value);

            // Acessar valor do dicionario
            int key = item.Key;
            String value = json[key];
            System.Console.WriteLine(value);
        }

        Console.Read();
    }
}