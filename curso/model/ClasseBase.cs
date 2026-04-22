public class ClasseBase
{
    // A variavel virtual serve para liberar polimorfismo do método
    // Ao contrario do java, que só barra polimorfismo se tiver o final
    public virtual void Metodo()
    {
        Console.WriteLine("Método classe base");
    }
}