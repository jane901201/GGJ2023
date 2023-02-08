using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSceneObject : BaseSceneObject
{
    public override SceneObjectType ObjectType { get => SceneObjectType.Obstacle; }

    public int Hardness = 1;
    public int DownLife = 0;
    
}
