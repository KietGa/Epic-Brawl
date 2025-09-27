using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject brawlers;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject models;
    [SerializeField] private float rotateSpeed = 1.2f;
    public TextMeshProUGUI skillBrawlText;
    public GameObject[] brawlModels;
    public string[] skillBrawls { get; set; }
    public int brawlPicked;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        skillBrawls = new string[12];

        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (DataManager.DMInstance != null)
        {
            Destroy(DataManager.DMInstance.gameObject);
        }

        if (OnlineLobbyManager.OLMInstace != null)
        {
            Destroy(OnlineLobbyManager.OLMInstace.gameObject);
        }
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Brawlers()
    {
        SkillDetail();
        skillBrawlText.text = skillBrawls[brawlPicked];
        brawlPicked = 0;
        brawlModels[brawlPicked].SetActive(true);
        menu.SetActive(false);
        models.SetActive(true);
        brawlers.SetActive(true);
    }

    public void BackBrawlers()
    {
        SetBrawl(0);
        menu.SetActive(true);
        models.SetActive(false);
        brawlers.SetActive(false);
    }

    public void Tutorial() 
    {
        menu.SetActive(false);
        tutorial.SetActive(true);
    }

    public void BackTutorial()
    {
        menu.SetActive(true);
        tutorial.SetActive(false);
    }

    public void SetBrawl(int brawlId)
    {
        brawlModels[brawlPicked].SetActive(false);
        brawlPicked = brawlId;
        brawlModels[brawlId].SetActive(true);
        skillBrawlText.text = skillBrawls[brawlPicked];
    }

    private void SkillDetail()
    {
        skillBrawls[0] = "August (Controller)\r\n\r\nHP: 6000 - Range: 100\r\nSPD: 16 - Reload SPD: 3s\r\n\r\nAttack: Shoot a single bullet deal 2000 damage and charge 1 CP\r\n\r\nCD Skill (16s): Instantly reload 1 ammo, charge 1 CP and heal 1000 hp\r\n\r\nCharged Skill (4CP): Throw a grenade deal 3000 damage when it exploded";
        skillBrawls[1] = "Bang (Damage Dealer)\r\n\r\nHP: 5000 - Range: 90\r\nSPD: 16 - Reload SPD: 4s\r\n\r\nAttack: Shoot rapidly 6 bullets each deal 600 damage and charge 2 CP\r\n\r\nCD Skill (18s): Instantly reload 2 ammos\r\n\r\nCharged Skill (24CP): Shoot rapidly 12 bullets with extra 15 range each deal 600 damage and charge 1 CP \r\n";
        skillBrawls[2] = "Barou (Artillery)\r\n\r\nHP: 5000 - Range: ?\r\nSPD: 16 - Reload SPD: 4s\r\n\r\nAttack: Throw a grenade deal 2000 damage when it explode and charge 1 CP per enemy\r\n\r\nCD Skill (18s): Instantly heal 1500 hp\r\n\r\nCharged Skill (4CP): Throw a big grenade deal 4000 damage and explode larger and charge 1 CP per enemy (can hold longer)\r\n";
        skillBrawls[3] = "Bling (Marksman)\r\n\r\nHP: 4500 - Range: 120\r\nSPD: 14 - Reload SPD: 4s\r\n\r\nAttack: Snipe a bullet deal 2000 damage and charge 1 CP\r\n\r\nCD Skill (12s): Dash backward 20m\r\n\r\nCharged Skill (5CP): Shoot a VIP bullet go farther extra 30 range and deal 4000 damage";
        skillBrawls[4] = "Eve (Duelist)\r\n\r\nHP: 6000 - Range: 100\r\nSPD: 18 - Reload SPD: 2s\r\n\r\nAttack: Shoot a single bullet deal 1500 damage and charge 1 CP\r\n\r\nCD Skill (15s): Speed up to 26 SPD for 5s\r\n\r\nCharged Skill (2CP): Dash 16m base on Movement";
        skillBrawls[5] = "Shin (Assassin)\r\n\r\nHP: 7500 - Range: 10\r\nSPD: 26 - Reload SPD: 2s\r\n\r\nAttack: Swing a knife deal 2500 damage and charge 1 CP\r\n\r\nCD Skill (6s): Throw a knife deal 2500 damage and charge 2 CP\r\n\r\nCharged Skill (5CP): Invisibility and speed up to 30 SPD for 5s";
        skillBrawls[6] = "Shogun (Fighter)\r\n\r\nHP: 9000 - Range: 40\r\nSPD: 22 - Reload SPD: 3s\r\n\r\nAttack: Shoot 5 bullets each deal 500 damage and charge 1 CP\r\n\r\nCD Skill (10s): Dash forward 20m\r\n\r\nCharged Skill (10CP): Shoot 10 bullets each go farther 10m, deal 600 damage and charge 1 CP";
        skillBrawls[7] = "Tusk (Vanguard)\r\n\r\nHP: 11000 - Range: 10\r\nSPD: 24 - Reload SPD: 2s\r\n\r\nAttack: Swing a axe deal 2000 damage and charge 1 CP\r\n\r\nCD Skill (22s): Speed up to 36 SPD and Attack damage inscrese to 2500 for 5s\r\n\r\nCharged Skill (2CP): Instantly heal 5000 HP";
        skillBrawls[8] = "Reo (Slayer)\r\n\r\nHP: 5500 - Range: 100\r\nSPD: 18 - Reload SPD: 2s\r\n\r\nAttack:Shoot a single bullet deal 1600 damage and charge 1 CP\r\n\r\nCD Skill (8s): Swing a knife deal 3000 damage and charge 2 CP\r\n\r\nCharged Skill (6CP): Become smaller (= 1/4 original scale) and inscrese speed to 26 for 6s";
        skillBrawls[9] = "Touka (Healer)\r\n\r\nHP: 6000 - Range: 100\r\nSPD: 18 - Reload SPD: 3s\r\n\r\nAttack: Fire a spell that deal 1800 damage and charge 1 CP\r\n\r\nCD Skill (12s): Heal 2000 HP to 1 alliy Touka target and charge 1 CP\r\n\r\nCharged Skill (6CP): Heal all allies (include Touka) 2000 HP";
        skillBrawls[10] = "Nagi (Mage)\r\n\r\nHP: 5000 - Range: 100\r\nSPD: 16 - Reload SPD: 3s\r\n\r\nAttack: Fire a spell that deal 1800 damage and charge 1 CP\r\n\r\nCD Skill (10s): Fire a spell that deal 3000 damage and charge 2 CP\r\n\r\nCharged Skill (10CP): Cast a spell that deal 2000 damage to ALL enemy";
        skillBrawls[11] = "Fara (Battlemage)\r\n\r\nHP: 6500 - Range: 100\r\nSPD: 16 - Reload SPD: 3s\r\n\r\nAttack: Fire a spell that deal 1800 damage and charge 1 CP\r\n\r\nCD Skill (12s): Fire a spell that deal 1500 damage, heal 1500 HP and charge 1 CP\r\n\r\nCharged Skill (4CP): Fire a spell that deal 1500 damage, heal 1500 HP and charge 1 CP";
    }
}
