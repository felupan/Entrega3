using System;
using Player;
using ScriptableObjects;
using UnityEngine;

public class PlayerWeaponSystem : PlayerSystem
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private InputReaderSO inputReader;
    public GameObject CurrentWeapon => weapons[currentIndex];
    
    private int currentIndex;

    private void OnEnable()
    {
        inputReader.OnWeaponSwitch += SwitchWeapon;
    }

    private void OnDisable()
    {
        inputReader.OnWeaponSwitch -= SwitchWeapon;
    }

    private void SwitchWeapon(int index)
    {
        weapons[currentIndex].gameObject.SetActive(false);
        currentIndex = index;
        weapons[currentIndex].gameObject.SetActive(true);
    }
}
