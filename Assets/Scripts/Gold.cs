using UnityEngine;
using System.Collections;

public class Gold : ScriptableObject {

    public float amount;

    public static Gold create(float amount)
    {
        Gold gold = CreateInstance<Gold>();
        gold.amount = amount;
        return gold;
    }

    public override string ToString()
    {
        return amount.ToString();
    }

    public override bool Equals(object o)
    {
        return base.Equals(o);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static Gold operator +(Gold c1, Gold c2)
    {
        return Gold.create(c1.amount + c2.amount);
    }

    public static Gold operator -(Gold c1, Gold c2)
    {
        return Gold.create(c1.amount - c2.amount);
    }

    public static Gold operator /(Gold c1, Gold c2)
    {
        return Gold.create(c1.amount / c2.amount);
    }

    public static Gold operator *(Gold c1, Gold c2)
    {
        return Gold.create(c1.amount * c2.amount);
    }

    public static bool operator <(Gold c1, Gold c2)
    {
        return c1.amount < c2.amount;
    }

    public static bool operator >(Gold c1, Gold c2)
    {
        return c1.amount > c2.amount;
    }

    public static bool operator <=(Gold c1, Gold c2)
    {
        return c1.amount <= c2.amount;
    }

    public static bool operator >=(Gold c1, Gold c2)
    {
        return c1.amount >= c2.amount;
    }

    public static bool operator ==(Gold c1, Gold c2)
    {
        return c1.amount == c2.amount;
    }

    public static bool operator !=(Gold c1, Gold c2)
    {
        return c1.amount != c2.amount;
    }

    public static bool operator <(Gold c1, float c2)
    {
        return c1.amount < c2;
    }

    public static bool operator >(Gold c1, float c2)
    {
        return c1.amount > c2;
    }

    public static bool operator <=(Gold c1, float c2)
    {
        return c1.amount <= c2;
    }

    public static bool operator >=(Gold c1, float c2)
    {
        return c1.amount >= c2;
    }

    public static bool operator ==(Gold c1, float c2)
    {
        return c1.amount == c2;
    }

    public static bool operator !=(Gold c1, float c2)
    {
        return c1.amount != c2;
    }
}
