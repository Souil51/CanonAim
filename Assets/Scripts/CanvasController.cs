using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private Sprite m_sprt_0;
    [SerializeField] private Sprite m_sprt_1;
    [SerializeField] private Sprite m_sprt_2;
    [SerializeField] private Sprite m_sprt_3;
    [SerializeField] private Sprite m_sprt_4;
    [SerializeField] private Sprite m_sprt_5;
    [SerializeField] private Sprite m_sprt_6;
    [SerializeField] private Sprite m_sprt_7;
    [SerializeField] private Sprite m_sprt_8;
    [SerializeField] private Sprite m_sprt_9;

    public void UpdateScore(int nScore)
    { 
        Transform tScore = transform.Find("PanelScore");
        Transform tScoreText = tScore.Find("Score");
        tScoreText.GetComponent<Text>().text = nScore.ToString();
    }

    public void UpdateCanonBulletCount(int nBulletCount)
    {
        Transform tBullet = transform.Find("PanelBullet");
        Transform tBulletScore = tBullet.Find("BulletCount");
        tBulletScore.GetComponent<Text>().text = nBulletCount.ToString();
    }

    public void SetWaitingBulletSliderPosition(Vector3 vPos)
    {
        Transform tWaitingBullet = transform.Find("WatingBulletSlider");
        RectTransform rTrans = tWaitingBullet.GetComponent<RectTransform>();
        rTrans.position = new Vector3(vPos.x, vPos.y - 60, vPos.z);
    }

    public void UpdateWaitingBulletSlider(float fValue)
    {
        if (fValue < 0)
            fValue = 0;
        else if (fValue > 1)
            fValue = 1;

        Transform tWaitingBullet = transform.Find("WatingBulletSlider");
        tWaitingBullet.GetComponent<Slider>().value = fValue;

        Transform tFillArea = tWaitingBullet.Find("Fill Area");
        Transform tFill = tFillArea.Find("Fill");
        tFill.GetComponent<Image>().color = new Color(1, 1 - fValue, 1 - fValue);
    }

    public void UpdateEndScreenScore(int nScore)
    {
        int nCentaines = (nScore / 100) % 10;
        int nDizaines = (nScore / 10) % 10;
        int nUnites = nScore % 10;

        SetUnitImageSprite(transform.Find("PanelEnd").Find("100").GetComponent<Image>(), nCentaines);
        SetUnitImageSprite(transform.Find("PanelEnd").Find("10").GetComponent<Image>(), nDizaines);
        SetUnitImageSprite(transform.Find("PanelEnd").Find("1").GetComponent<Image>(), nUnites);
    }

    public void SetUnitImageSprite(Image img, int nUnit)
    {
        switch (nUnit)
        {
            case 0:
                img.sprite = m_sprt_0;
                break;
            case 1:
                img.sprite = m_sprt_1;
                break;
            case 2:
                img.sprite = m_sprt_2;
                break;
            case 3:
                img.sprite = m_sprt_3;
                break;
            case 4:
                img.sprite = m_sprt_4;
                break;
            case 5:
                img.sprite = m_sprt_5;
                break;
            case 6:
                img.sprite = m_sprt_6;
                break;
            case 7:
                img.sprite = m_sprt_7;
                break;
            case 8:
                img.sprite = m_sprt_8;
                break;
            case 9:
                img.sprite = m_sprt_9;
                break;
        }
    }

    public void StartGame()
    {
        transform.Find("PanelStart").gameObject.SetActive(false);
        transform.Find("PanelEnd").gameObject.SetActive(false);
    }

    public void EndGame()
    {
        transform.Find("PanelStart").gameObject.SetActive(false);
        transform.Find("PanelEnd").gameObject.SetActive(true);
    }
}
