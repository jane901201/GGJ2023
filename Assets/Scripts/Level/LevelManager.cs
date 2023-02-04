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
            // TODO: get current player position
            var currentPlayerPos = new Vector2(0f, 0f);
            var grid = _ConvertToGridUnit(currentPlayerPos);
            Debug.Log(grid);

            var minBound = grid - _generatedBounds;
            minBound.y = Mathf.Max(0, minBound.y);
            var maxBound = grid + _generatedBounds;
            Debug.Log(minBound);
            Debug.Log(maxBound);

            int xMin = minBound.x;
            int xMax = maxBound.x;

            _UpdateRange(maxBound.y);

            bool hasGenerated = false;

            for (int yMin = minBound.y ; yMin <= maxBound.y ; yMin++)
            {
                var yMinGeneratedRange = _generatedRange[yMin];
                int leftMinBound = yMinGeneratedRange.x;
                int rightMaxBound = yMinGeneratedRange.y;

                for (int yMax = yMin ; yMax <= maxBound.y ; yMax++)
                {
                    var yMaxGeneratedRange = _generatedRange[yMax];
                    leftMinBound = Mathf.Min(leftMinBound, yMaxGeneratedRange.x);
                    rightMaxBound = Mathf.Max(rightMaxBound, yMaxGeneratedRange.y);

                    if (leftMinBound == rightMaxBound)
                    {
                        Debug.LogFormat("({0},{1}) <-> ({2},{3})", xMin, yMin, xMax, yMax);

                        var sceneObject = _sceneObjectSetting.RandomPick(grid.y, new Vector2Int(xMax-xMin+1, yMax-yMin+1));

                        if (sceneObject != null)
                        {
                            var obj = GameObject.Instantiate(sceneObject.Prefab);
                            obj.transform.position = new Vector3(leftMinBound+sceneObject.Size.x/2f, yMin) * _unitLength;

                            leftMinBound = leftMinBound - 1;
                            rightMaxBound = rightMaxBound + sceneObject.Size.x;

                            for (int y = yMin ; y <= yMax ; y++)
                            {
                                // set yMin to yMax 's range to leftMinBound, rightMaxBound
                                _generatedRange[y] = new Vector2Int(leftMinBound, rightMaxBound);
                            }
                            hasGenerated = true;
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogFormat("({0},{1}) <-> ({2},{3})", xMin, yMin, leftMinBound, yMax);
                        Debug.LogFormat("({0},{1}) <-> ({2},{3})", rightMaxBound, yMin, xMax, yMax);

                        var sceneObject1 = _sceneObjectSetting.RandomPick(grid.y, new Vector2Int(leftMinBound-xMin+1, yMax-yMin+1));
                        if (sceneObject1 != null)
                        {
                            var obj = GameObject.Instantiate(sceneObject1.Prefab);
                            obj.transform.position = new Vector3(leftMinBound+sceneObject1.Size.x/2f+1f, yMin) * _unitLength;

                            leftMinBound = leftMinBound - sceneObject1.Size.x;

                            for (int y = yMin ; y <= yMax ; y++)
                            {
                                _generatedRange[y] = new Vector2Int(leftMinBound, _generatedRange[y].y);
                            }
                            hasGenerated = true;
                            break;
                        }
                        Debug.Log(sceneObject1?.Prefab);

                        var sceneObject2 = _sceneObjectSetting.RandomPick(grid.y, new Vector2Int(xMax-rightMaxBound+1, yMax-yMin+1));
                        Debug.Log(sceneObject2?.Prefab);
                        if (sceneObject2 != null)
                        {
                            var obj = GameObject.Instantiate(sceneObject2.Prefab);
                            obj.transform.position = new Vector3(rightMaxBound-sceneObject2.Size.x/2f-1f, yMin) * _unitLength;

                            rightMaxBound = rightMaxBound + sceneObject2.Size.x;

                            for (int y = yMin ; y <= yMax ; y++)
                            {
                                _generatedRange[y] = new Vector2Int(_generatedRange[y].x, rightMaxBound);
                            }
                            hasGenerated = true;
                            break;
                        }
                    }
                }
                if (hasGenerated)
                {
                    break;
                }
            }

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    break;
                }
                yield return null;
            }

            yield return null;
        }
    }

    private void _UpdateRange(int maxY)
    {
        if (_generatedRange.Count < maxY+1)
        {
            _generatedRange.AddRange(Enumerable.Range(1, maxY + 1 - _generatedRange.Count).Select(_ => new Vector2Int()));
        }
    }

    private Vector2Int _ConvertToGridUnit(Vector2 pos)
    {
        return new Vector2Int((int)(pos.x/_unitLength), -(int)(pos.y/_unitLength));
    }
}
