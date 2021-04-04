using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Michsky.UI.ModernUIPack;

public class PhaseDropdown : MonoBehaviour
{
    [SerializeField]
    Sprite dropdownIcon = default;
    //Phase選択するDropDownにGamePhaseの項目をセットする。
    private void Start() {
        var dropdown = GetComponent<CustomDropdown>();
        if(dropdown == null){
            Debug.LogAssertion("Could not find Dropdown Component!");
        }
        foreach(GamePhase value in Enum.GetValues(typeof(GamePhase))){
            dropdown.CreateNewItemFast(value.ToString(), dropdownIcon);
        }
        dropdown.SetupDropdown();
        dropdown.dropdownEvent.AddListener(SelectPreferPhase);
        Debug.Log("Completed to set up Dropdown options.");
    }
    public void SelectPreferPhase(int targetPhaseNum){
        FindObjectOfType<CommonManager>().LoadPhase(targetPhaseNum);
    }
}
