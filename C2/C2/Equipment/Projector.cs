namespace C2;

public class Projector : Equipment
{
    private int lumens {set;get;}
    private int lampPower {set;get;}

    public Projector(string producer, string name, int lumens, int lampPower) : base(producer, name)
    {
        this.lumens = lumens;
        this.lampPower = lampPower;
    }
}