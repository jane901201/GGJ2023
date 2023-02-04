using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private SceneObjectSetting _sceneObjectSetting;

    [SerializeField] private List<Vector2Int> _generatedRange = new List<Vector2Int>();
    [SerializeField] private float _unitLength = 100f;
    [SerializeField] private Vector2Int _generatedBounds = new Vector2Int(5, 10);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(_AutoGenerate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator _AutoGenerate()
    {
        while (true)
        {
            var currentPlayerPos = new Vector2(0f, 0f);
            var grid = _ConvertToGridUnit(currentPlayerPos);
            Debug.Log(grid);
            var minBound = grid - _generatedBounds;
            minBound.y = Mathf.Max(0, minBound.y);
            var maxBound = grid + _generatedBounds;
            Debug.Log(minBound);
            Debug.Log(maxBound);

            for (int yMin = minBound.y ; yMin <= maxBound.y ; yMin++)
            {
                var range = _GetRangeByY(yMin);
                int leftMinBound = range.x;
                int rightMaxBound = range.y;
                int xMin = minBound.x;
                int xMax = maxBound.x;

                if (range.x == range.y)
                {
                    for (int yMax = yMin ; yMax <= maxBound.y ; yMax++)
                    {
                        var maxRange = _GetRangeByY(yMax);
                        leftMinBound = Mathf.Min(maxRange.x, leftMinBound);
                        rightMaxBound = Mathf.Max(maxRange.y, rightMaxBound);

                        Debug.LogFormat("({0},{1}) <-> ({2},{3})", xMin, yMin, xMax, yMax);

                        var sceneObject = _sceneObjectSetting.RandomPick(grid.y, new Vector2Int(xMax-xMin+1, yMax-yMin+1));
                        Debug.Log(sceneObject.Prefab);
                    }
                }
                else
                {
                    for (int yMax = yMin ; yMax <= maxBound.y ; yMax++)
                    {
                        var maxRange = _GetRangeByY(yMax);
                        leftMinBound = Mathf.Min(maxRange.x, leftMinBound);
                        rightMaxBound = Mathf.Max(maxRange.y, rightMaxBound);

                        Debug.LogFormat("({0},{1}) <-> ({2},{3})", xMin, yMin, leftMinBound, yMax);
                        Debug.LogFormat("({0},{1}) <-> ({2},{3})", rightMaxBound, yMin, xMax, yMax);

                        var sceneObject = _sceneObjectSetting.RandomPick(grid.y, new Vector2Int(xMax-xMin+1, yMax-yMin+1));
                    }
                }
            }

            yield return null;
        }
    }

    private Vector2Int _GetRangeByY(int y)
    {
        if (_generatedRange.Count < y+1)
        {
            _generatedRange.AddRange(Enumerable.Range(1, y + 1 - _generatedRange.Count).Select(_ => new Vector2Int()));
        }
        return _generatedRange[y];
    }

    private Vector2Int _ConvertToGridUnit(Vector2 pos)
    {
        return new Vector2Int((int)(pos.x/_unitLength), -(int)(pos.y/_unitLength));
    }
}
