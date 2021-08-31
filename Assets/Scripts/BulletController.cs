using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float m_fBasketYLimit;

    public delegate void BulletYLimitHandler(object sender, EventArgs args);
    public event BulletYLimitHandler BulletYLimit;

    private bool m_bIsEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < m_fBasketYLimit && m_bIsEnabled)
        {
            BulletYLimit?.Invoke(this, EventArgs.Empty);
            m_bIsEnabled = false;
        }
    }

    public void DisableBullet()
    {
        m_bIsEnabled = false;
    }
}
