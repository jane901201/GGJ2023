using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Tilemap m_Tilemap;
    [SerializeField] private int m_TopTilemap = 4;
    [SerializeField] private int m_ButtonTilemap = -4;
    [SerializeField] private float shootCd = .5f;

    private Vector3Int m_CurrentPlayerPosition;

    public bool IsCd { get; private set; }


    private void Start()
    {
        m_CurrentPlayerPosition = new Vector3Int(-9, 0, 0);
        transform.position = m_Tilemap.GetCellCenterWorld(m_CurrentPlayerPosition);
    }

    public void MoveUp()
    {
        if (m_CurrentPlayerPosition.y < m_TopTilemap)
        {
            m_CurrentPlayerPosition = m_CurrentPlayerPosition + new Vector3Int(0, 1, 0);
            transform.position = m_Tilemap.GetCellCenterWorld(m_CurrentPlayerPosition);
        }
    }

    public void MoveDown()
    {
        if (m_CurrentPlayerPosition.y > m_ButtonTilemap)
        {
            m_CurrentPlayerPosition = m_CurrentPlayerPosition + new Vector3Int(0, -1, 0);
            transform.position = m_Tilemap.GetCellCenterWorld(m_CurrentPlayerPosition);
        }
    }

    private IEnumerator CoolDown()
    {
        IsCd = true;
        yield return new WaitForSeconds(shootCd);
        IsCd = false;
    }
}
