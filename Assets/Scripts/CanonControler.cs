using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonControler : MonoBehaviour
{
    [SerializeField] private GameObject m_readySprite;
    [SerializeField] private GameObject m_loadingSprite;
    [SerializeField] private float m_fRotationRate;
    [SerializeField] private float m_fRotationMax;
    [SerializeField] private float m_fRotationMin;
    [SerializeField] private float m_fDotsSpacing;
    [SerializeField] private float m_fDotsScaleFactor;
    [SerializeField] private float m_fForce;
    [SerializeField] private int m_nBulletCount;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private float m_fWaitingDuration;
    [SerializeField] private float m_fCanonSpeedFactor;//Le canon monte et descend de plus en plus vite
    [SerializeField] private float m_fTrajectoryVisibleFactor;//La trajectoire est de moins en moins visible

    public delegate void BulletShotHandler(object sender, EventArgs args);
    public event BulletShotHandler BulletShot;

    public delegate void CanonBulletYLimitHandler(object sender, EventArgs args);
    public event CanonBulletYLimitHandler CanonBulletYLimit;

    private CanvasController m_canvasCtrl;

    private int m_nRotationDirection = 1;
    private bool m_bWaitingBullet = false;
    private float m_fCurrentWaitingDuration = 0f;
    private float m_fStartingDotsSpacing;
    private float m_fStartingRotationRate;

    private readonly GameObject[] m_listDots = new GameObject[10];

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + (m_fRotationRate * m_nRotationDirection));

        if (transform.eulerAngles.z < m_fRotationMin)
        {
            m_nRotationDirection *= -1;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, m_fRotationMin);
        }

        if(transform.eulerAngles.z > m_fRotationMax)
        {
            m_nRotationDirection *= -1;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, m_fRotationMax);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !m_bWaitingBullet && !GameManager.GetInstance().IsGameEnded() && GameManager.GetInstance().IsGameStarted())
        {
            if (m_nBulletCount > 0)
            {
                float xPos = 1 * Mathf.Cos((transform.eulerAngles.z) * Mathf.Deg2Rad) + transform.position.x;
                float yPos = 1 * Mathf.Sin((transform.eulerAngles.z) * Mathf.Deg2Rad) + transform.position.y;
                GameObject goBullet = (GameObject)Instantiate(Resources.Load("bullet"));
                BulletController bltController = goBullet.GetComponent<BulletController>();
                bltController.BulletYLimit += BltController_BulletYLimit;

                goBullet.transform.position = new Vector3(xPos, yPos, 0);

                Rigidbody2D rb2d = goBullet.GetComponentInParent<Rigidbody2D>();
                float xForce = Mathf.Cos((transform.eulerAngles.z) * Mathf.Deg2Rad);
                float yForce = Mathf.Sin((transform.eulerAngles.z) * Mathf.Deg2Rad);

                rb2d.AddForce(new Vector2(xForce, yForce) * m_fForce, ForceMode2D.Impulse);

                m_nBulletCount--;
                BulletShot?.Invoke(this, EventArgs.Empty);

                UpdateRotationRate(m_fCanonSpeedFactor);//Vitesse +X%
                UpdateDotSpacing(m_fTrajectoryVisibleFactor);//Réduction de la taille de la trajectoire -X%
            }

            GameManager.GetInstance().PlaySound(GameManager.Sound.LOADING);
            m_canvasCtrl.UpdateWaitingBulletSlider(0);
            m_fCurrentWaitingDuration = 0f;
            m_bWaitingBullet = true;
            m_readySprite.SetActive(false);
            m_loadingSprite.SetActive(true);
        }

        float xForceTrajectory = Mathf.Cos((transform.eulerAngles.z) * Mathf.Deg2Rad) * m_fForce;
        float yForceTrajectory = Mathf.Sin((transform.eulerAngles.z) * Mathf.Deg2Rad) * m_fForce;

        float xPosition = 2.2f * Mathf.Cos((transform.eulerAngles.z) * Mathf.Deg2Rad) + transform.position.x;
        float yPosition = 2.2f * Mathf.Sin((transform.eulerAngles.z) * Mathf.Deg2Rad) + transform.position.y;

        for (int i = 0; i < m_listDots.Length; i++)
        {
            float fGravity = Physics2D.gravity.magnitude * Mathf.Pow((float)i / m_fDotsSpacing, 2);
            m_listDots[i].transform.position = new Vector3(xPosition + xForceTrajectory * ((float)i / m_fDotsSpacing), (yPosition + yForceTrajectory * ((float)i / m_fDotsSpacing)) - fGravity / 2f, 0);
        }

        if (m_bWaitingBullet)
        {
            m_fCurrentWaitingDuration += Time.deltaTime;

            float fSliderValue = m_fCurrentWaitingDuration / m_fWaitingDuration;
            m_canvasCtrl.UpdateWaitingBulletSlider(fSliderValue);

            if (m_fCurrentWaitingDuration >= m_fWaitingDuration)
            {
                m_bWaitingBullet = false;
                m_fCurrentWaitingDuration = 0f;
                m_readySprite.SetActive(true);
                m_loadingSprite.SetActive(false);
            }
        }
    }

    private void BltController_BulletYLimit(object sender, EventArgs args)
    {
        CanonBulletYLimit?.Invoke(this, EventArgs.Empty);
    }

    private void Awake()
    {
        m_fStartingDotsSpacing = m_fDotsSpacing;
        m_fStartingRotationRate = m_fRotationRate;

        m_canvasCtrl = m_canvas.GetComponent<CanvasController>();

        for (int i = 0; i < m_listDots.Length; i++)
        {
            m_listDots[i] = (GameObject)Instantiate(Resources.Load("TrajectoryDot"));
            m_listDots[i].transform.localScale = new Vector3(0.1f - (i * m_fDotsScaleFactor), 0.1f - (i * m_fDotsScaleFactor), 1);
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        m_canvasCtrl.SetWaitingBulletSliderPosition(screenPos);

        m_fCurrentWaitingDuration = m_fWaitingDuration;
        m_canvasCtrl.UpdateWaitingBulletSlider(1f);

        m_readySprite.SetActive(true);
        m_loadingSprite.SetActive(false);
    }

    public int GetBulletCount()
    {
        return m_nBulletCount;
    }

    public void AddBullet()
    {
        m_nBulletCount++;
    }

    private void UpdateRotationRate(float fValue)
    {
        m_fRotationRate *= fValue;

        if (m_fRotationRate > 0.75f)
            m_fRotationRate = 0.75f;
    }

    private void UpdateDotSpacing(float fValue)
    {
        m_fDotsSpacing *= fValue;

        if (m_fDotsScaleFactor > 15)
            m_fDotsScaleFactor = 15f;
    }

    public void ResetCanon()
    {
        m_nBulletCount = 10;

        m_bWaitingBullet = false;
        m_fCurrentWaitingDuration = 0f;
        m_readySprite.SetActive(true);
        m_loadingSprite.SetActive(false);
        m_fDotsSpacing = m_fStartingDotsSpacing;
        m_fRotationRate = m_fStartingRotationRate;
    }
}
