﻿using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChooseSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button btn;
    private BasicSkill skill;
    private CombatButton target;
    private GameObject hoverBlock;
    private TextMeshProUGUI hoverText;
    public Image img;

    // Start is called before the first frame update
    private void Start()
    {
        btn.onClick.AddListener(Click);
    }

    public void Click()
    {
        target.skill = skill;
        target.Setup();
    }

    public void Setup(BasicSkill basicSkill, CombatButton combatButton,GameObject hover,TextMeshProUGUI text)
    {
        skill = basicSkill;
        target = combatButton;
        hoverBlock = hover;
        hoverText = text;
        img.sprite = skill.Icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverBlock.SetActive(true);
        hoverText.text = $"{skill.Title}";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverBlock.SetActive(false);
    }
}