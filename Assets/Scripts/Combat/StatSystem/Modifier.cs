public abstract class Modifier
{
    public abstract float Apply(float value);
}

public class FlatModifier : Modifier
{
    public float Amount;
    public FlatModifier(float amount) => Amount = amount;
    public override float Apply(float value) => value + Amount;
}

public class PercentModifier : Modifier
{
    public float Percent;
    public PercentModifier(float percent) => Percent = percent;
    public override float Apply(float value) => value * (1 + Percent);
}
