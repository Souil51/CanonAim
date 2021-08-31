using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletEnterEventArgs
{
    public int Points { get; } = 0;

    public BulletEnterEventArgs(int nPoints)
    {
        Points = nPoints;
    }
}

public class BoxColliderController : MonoBehaviour
{
    [SerializeField] private Sprite m_sprtTile_1;
    [SerializeField] private Sprite m_sprtTile_2;
    [SerializeField] private Sprite m_sprtTile_3;
    [SerializeField] private Transform m_tMurDroit;
    [SerializeField] private Transform m_tMurGauche;
    [SerializeField] private float m_fBoxMaxSize;
    [SerializeField] private float m_fBoxMinSize;
    [SerializeField] private float m_fBoxSizeChangeRate;

    private readonly Dictionary<int, BoxColliderSize> m_dicSize = new Dictionary<int, BoxColliderSize>()
    {
        { 3, new BoxColliderSize(0.64f, -0.42f, 0.42f)},
        { 2, new BoxColliderSize(1.14f, -0.67f, 0.67f)},
        { 1, new BoxColliderSize(1.64f, -0.92f, 0.92f)}
    };

    public delegate void BulletEnterHandler(object sender, BulletEnterEventArgs args);
    public event BulletEnterHandler BulletEnter;

    private int m_nPoints;
    private Transform m_tImage;
    private SpriteRenderer m_sprtRendererPointsText;
    private BoxCollider2D m_boxCollider;
    
    private void Awake()
    {
        m_tImage = transform.Find("PointsText");
        m_sprtRendererPointsText = m_tImage.GetComponent<SpriteRenderer>();
        m_boxCollider = transform.GetComponent<BoxCollider2D>();

        SetRandomPoints();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            BulletEnterEventArgs args = new BulletEnterEventArgs(m_nPoints);
            BulletEnter?.Invoke(this, args);
            SetRandomPoints();

            collision.gameObject.GetComponent<BulletController>().DisableBullet();
        }
    }

    private void SetRandomPoints()
    {
        int nRand;
        do
        {
            nRand = Random.Range(1, 4);
        }
        while (nRand == m_nPoints);

        m_nPoints = nRand;

        switch (m_nPoints)
        {
            case 1:
                m_sprtRendererPointsText.sprite = m_sprtTile_1;
                break;
            case 2:
                m_sprtRendererPointsText.sprite = m_sprtTile_2;
                break;
            case 3:
                m_sprtRendererPointsText.sprite = m_sprtTile_3;
                break;
        }

        m_boxCollider.size = new Vector2(m_dicSize[m_nPoints].m_fBoxSize, m_boxCollider.size.y);
        m_tMurGauche.localPosition = new Vector3(m_dicSize[m_nPoints].m_fMurGauchePos, m_tMurGauche.localPosition.y, m_tMurGauche.localPosition.z);
        m_tMurDroit.localPosition = new Vector3(m_dicSize[m_nPoints].m_fMurDroitPos, m_tMurDroit.localPosition.y, m_tMurGauche.localPosition.z);
    }

    public int GetPoints()
    {
        return m_nPoints;
    }
}

public class BoxColliderSize
{
    public float m_fBoxSize;
    public float m_fMurGauchePos;
    public float m_fMurDroitPos;

    public BoxColliderSize(float fBoxSoze, float fMurGauche, float fMurDroit)
    {
        m_fBoxSize = fBoxSoze;
        m_fMurGauchePos = fMurGauche;
        m_fMurDroitPos = fMurDroit;
    }
}