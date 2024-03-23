
using System;
using UnityEngine;

public interface IHealth
{
    int Hp { get; }
    int MaxHp { get; }
    void Heal(int value);
    void Hurt(int value);
}

public interface IExperience
{
    int Exp { get; }
    int MaxExp { get; }
    void GainExp(int value);

    int Level { get; }
    int MaxLevel { get; }
}

public interface IHunger
{
    int Hunger { get;  }
    int MaxHunger { get; }
    void HungerInc(int value);
    void HungerDec(int value);
}

public abstract class CharacterBase : MonoBehaviour, IHealth, IExperience, IHunger
{
    public int Hp { get; protected set; }

    public int MaxHp { get; protected set; }

    public virtual void Heal(int value)
    {
        Hp = Math.Clamp(Hp + value, 0, MaxHp);
    }

    public virtual void Hurt(int value)
    {
        Hp = Math.Clamp(Hp - value, 0, MaxHp);
    }

    public int Exp { get; protected set; }

    public int MaxExp { get; protected set; }

    public int Level { get; protected set; }

    public int MaxLevel { get; protected set;}



    public virtual void GainExp(int value)
    {
        // TODO
    }

    public int Hunger { get; protected set; }

    public int MaxHunger { get; protected set; }

    public virtual void HungerInc(int value)
    {
        Hunger = Math.Clamp(Hunger + value, 0, MaxHunger);
    }

    public virtual void HungerDec(int value)
    {
        Hunger = Math.Clamp(Hunger - value, 0, MaxHunger);
    }
}