using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Sound { SHOT = 0, BULLET_IN_BASKET = 1, BULLET_NOT_IN_BASKET = 2, LOADING = 3, END_MUSIC = 4}

    [SerializeField] private float m_fBoxStartPosition;
    [SerializeField] private float m_fBoxEndPosition;
    [SerializeField] private float m_fBoxPosY;
    [SerializeField] private int m_nBoxCount;
    [SerializeField] private int m_bPointsToGetBullet;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private GameObject m_canon;
    [SerializeField] private AudioClip m_audioShot;
    [SerializeField] private AudioClip m_audioBulletInBasket;
    [SerializeField] private AudioClip m_audioBulletNotInBasket;
    [SerializeField] private AudioClip m_audioBulletLoading;
    [SerializeField] private AudioClip m_audioEndMusic;
    //Augmentation de la difficulté en fonction du nombre de lancer
    [SerializeField] private float m_fCloseBasketFactor;//Les se ferment de plus en plus

    private static GameManager instance;

    private CanvasController m_canvasCtrl;
    private CanonControler m_canonCtrl;

    private readonly List<BoxColliderController> m_lstBoxColliders = new List<BoxColliderController>();
    private int m_nScore = 0;
    private bool m_bGameEnded = false;
    private bool m_bGameStarted = false;

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        if (m_bGameEnded || !m_bGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }
        }
    }
    private void StartGame()
    {
        GameObject[] goBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach(GameObject go in goBullets)
        {
            Destroy(go);
        }

        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -Mathf.Abs(Physics2D.gravity.y));
        m_canonCtrl.ResetCanon();
        m_nScore = 0;
        m_canvasCtrl.UpdateScore(m_nScore);
        m_canvasCtrl.UpdateCanonBulletCount(m_canonCtrl.GetBulletCount());

        m_bGameStarted = true;
        m_bGameEnded = false;
        m_canvasCtrl.StartGame();
    }

    public bool IsGameEnded()
    {
        return m_bGameEnded;
    }

    public bool IsGameStarted()
    {
        return m_bGameStarted;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        m_canvasCtrl = m_canvas.GetComponent<CanvasController>();
        m_canonCtrl = m_canon.GetComponent<CanonControler>();

        m_canonCtrl.BulletShot += GameManager_BulletShot;
        m_canonCtrl.CanonBulletYLimit += M_canonCtrl_CanonBulletYLimit;

        m_canvasCtrl.UpdateCanonBulletCount(m_canonCtrl.GetBulletCount());

        float fInterval = (m_fBoxEndPosition - m_fBoxStartPosition) / (m_nBoxCount - 1);

        for(int i = 0; i < m_nBoxCount; i++)
        {
            GameObject go = (GameObject)Instantiate(Resources.Load("BoxCollider"));

            go.transform.position = new Vector3(m_fBoxStartPosition + (i * fInterval), m_fBoxPosY, 1);

            BoxColliderController boxCollider = go.GetComponent<BoxColliderController>();
            boxCollider.BulletEnter += BoxCollider_BulletEnter;

            m_lstBoxColliders.Add(boxCollider);
        }
    }

    private void M_canonCtrl_CanonBulletYLimit(object sender, System.EventArgs args)
    {
        PlaySound(Sound.BULLET_NOT_IN_BASKET);

        if (m_canonCtrl.GetBulletCount() == 0)
        {
            StartCoroutine("Coroutine_StopGame");
        }
    }

    private void GameManager_BulletShot(object sender, System.EventArgs args)
    {
        PlaySound(Sound.SHOT);

        m_canvasCtrl.UpdateCanonBulletCount(m_canonCtrl.GetBulletCount());

        if (m_canonCtrl.GetBulletCount() == 0)
            m_bGameEnded = true;
    }

    private void BoxCollider_BulletEnter(object sender, BulletEnterEventArgs args)
    {
        PlaySound(Sound.BULLET_IN_BASKET);

        BoxColliderController boxColliderController = (BoxColliderController)sender;
        m_nScore += boxColliderController.GetPoints();

        m_canvasCtrl.UpdateScore(m_nScore);

        if(args.Points >= m_bPointsToGetBullet)
        {
            m_canonCtrl.AddBullet();
            m_canvasCtrl.UpdateCanonBulletCount(m_canonCtrl.GetBulletCount());

            m_bGameEnded = false;
        }

        if(m_canonCtrl.GetBulletCount() == 0)
        {
            StartCoroutine("Coroutine_StopGame");
        }
    }

    private IEnumerator Coroutine_StopGame()
    {
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y * -1);
        PlaySound(Sound.END_MUSIC);

        yield return new WaitForSeconds(2f);

        m_bGameEnded = true;
        m_canvasCtrl.UpdateEndScreenScore(m_nScore);
        m_canvasCtrl.EndGame();
    }

    public void PlaySound(Sound sound)
    {
        switch (sound) 
        {
            case Sound.SHOT: AudioSource.PlayClipAtPoint(m_audioShot, Vector3.zero); break;
            case Sound.BULLET_IN_BASKET: AudioSource.PlayClipAtPoint(m_audioBulletInBasket, Vector3.zero); break;
            case Sound.BULLET_NOT_IN_BASKET: AudioSource.PlayClipAtPoint(m_audioBulletNotInBasket, Vector3.zero); break;
            case Sound.LOADING: AudioSource.PlayClipAtPoint(m_audioBulletLoading, Vector3.zero, 0.25f); break;
            case Sound.END_MUSIC: AudioSource.PlayClipAtPoint(m_audioEndMusic, Vector3.zero); break;
        }
    }
}
