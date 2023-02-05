using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSceneObject : BaseSceneObject
{
    public override SceneObjectType ObjectType { get => SceneObjectType.Effect; }

    public int Life;
    public int Score;
    public int Hardness;

    public float SpeedChangeRatio;
    public float SpeedChangeDuration;

    public float LifeDownChangeRatio;
    public float LifeDownChangeDuration;


    public void Apply(GameplayPresenter gameplayPresenter)
    {
        gameplayPresenter.AddLife(Life);
        gameplayPresenter.AddEffectScore(Score);
        gameplayPresenter.AddHardness(Hardness);
        gameplayPresenter.AddEffect(new Effect()
        {
            Duration = SpeedChangeDuration,
            Value = SpeedChangeRatio,
        });
        gameplayPresenter.AddEffect(new Effect()
        {
            Duration = LifeDownChangeDuration,
            Value = LifeDownChangeRatio,
        });
    }
}

public class Effect
{
    public enum EffectType
    {
        SpeedChange,
        LifeDownChange,
    }

    public EffectType Type;
    public float Duration;
    public float Value;
}