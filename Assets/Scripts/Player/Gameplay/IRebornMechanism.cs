using UnityEngine;

public interface IRebornMechanism
{
    (Vector3 newDir, LineNode lineNode) GetDest();
}