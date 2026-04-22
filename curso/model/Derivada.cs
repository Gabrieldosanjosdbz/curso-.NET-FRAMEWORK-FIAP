class Derivada : ClasseBase //equivalente a extends do java, é o : 
{
    // equivalente ao super do java. Diferença é que o base vai na assinatura e o super no corpo
    public Derivada() : base() //também pode passar parametros no base
    {
        
    }

    // sobrecarga de métodos (criar dois métodos com o msm nome, mas com funções diferentes)
    public void funcaoGenerica()
    {
        Console.WriteLine("Sou uma função generica");
    }

    public void funcaoGenerica(String argumento)
    {
        Console.WriteLine("Sou a mesma função generica, mas com argumento: "+ argumento);
        //base.Metodo(); como pegar uma propriedade da classe pai, assim como é no java (super)
    }

    // Polimorfismo de um método só é permitido se no método pai tiver virtual, e a sobrescrita no filho tiver override
    // No java, utilizamos a anotação @Override
    public override void Metodo()
    {
        
    }
}