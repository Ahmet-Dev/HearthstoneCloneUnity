﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerPortraitVisual : MonoBehaviour {

    // TODO : get ID from players when game starts

    public GameObject Explosion;
    public CharacterAsset charAsset{ get; set;}
    [Header("Text Component References")]
    //public Text NameText;
    public Text HealthText;
    [Header("Image References")]
    public Image HeroPowerIconImage;
    public Image HeroPowerBackgroundImage;
    public Image PortraitImage;
    public Image PortraitBackgroundImage;

    public void ApplyLookFromAsset()
    {
        HealthText.text = charAsset.MaxHealth.ToString();
        HeroPowerIconImage.sprite = charAsset.HeroPowerIconImage;
        HeroPowerBackgroundImage.sprite = charAsset.HeroPowerBGImage;
        PortraitImage.sprite = charAsset.AvatarImage;
        PortraitBackgroundImage.sprite = charAsset.AvatarBGImage;

        HeroPowerBackgroundImage.color = charAsset.HeroPowerBGTint;
        PortraitBackgroundImage.color = charAsset.AvatarBGTint;

    }

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            DamageEffect.CreateDamageEffect(transform.position, amount);
            HealthText.text = healthAfter.ToString();
        }
    }

    public void Explode()
    {
        Instantiate(GlobalSettings.Instance.ExplosionPrefab, transform.position, Quaternion.identity);
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => GlobalSettings.Instance.GameOverCanvas.SetActive(true));
    }



}
