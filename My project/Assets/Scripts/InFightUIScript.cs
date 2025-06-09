using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class InFightUIScript : MonoBehaviour
{
    private GameManager gameManager;
    public SpriteRenderer spriteRendererBackround;
    public UnityEngine.UI.Image spriteRendererP;
    private GameObject player1icon;
    public UnityEngine.UI.Image spriteRendererAI;
    private GameObject player2icon;
    public Sprite[] backroundArray;
    public Sprite[] player1Array;
    public Sprite[] player2Array;
    void Awake()
    {
        spriteRendererBackround = GameObject.Find("Backround").GetComponent<SpriteRenderer>();
        spriteRendererP = GameObject.Find("F_Icon").GetComponent<UnityEngine.UI.Image>();
        spriteRendererAI = GameObject.Find("P2_Icon").GetComponent<UnityEngine.UI.Image>();
        gameManager = GameManager.Instance;
        player1icon = GameObject.FindGameObjectWithTag("IconPlayer");
        player2icon = GameObject.FindGameObjectWithTag("IconAI");
        SetUI();
    }

    public void SetUI()
    {
        ChooseMap();
        ChooseIcons();
    }
    public void ChooseMap()
    {
        if (gameManager.selectedMap == GameManager.MapChoice.Map1)
        {
            spriteRendererBackround.sprite = backroundArray[0];
        }
        else if (gameManager.selectedMap == GameManager.MapChoice.Map2)
        {
            spriteRendererBackround.sprite = backroundArray[1];
        }

    }
    public void ChooseIcons()
    {
        // ai icon -----------------------------------------
        if (gameManager.selectedAICharacter == GameManager.CharacterChoice.Tsuki)
        {
            spriteRendererAI.sprite = player2Array[0];
            ScaleIcons(player2icon, GameManager.CharacterChoice.Tsuki);
        }
        else if (gameManager.selectedAICharacter == GameManager.CharacterChoice.Mihu)
        {
            spriteRendererAI.sprite = player2Array[1];
            ScaleIcons(player2icon, GameManager.CharacterChoice.Mihu);
        }

        // -------------------------------------- player1 icon -----------------------------------------------
        if (gameManager.selectedPlayerCharacter == GameManager.CharacterChoice.Tsuki)
        {
            spriteRendererP.sprite = player1Array[0];
            ScaleIcons(player1icon, GameManager.CharacterChoice.Tsuki);
        }
        else if (gameManager.selectedPlayerCharacter == GameManager.CharacterChoice.Mihu)
        {
            spriteRendererP.sprite = player1Array[1];
            ScaleIcons(player1icon, GameManager.CharacterChoice.Mihu);
        }

    }
    // scale and place icons accordlingly so Tsuki's looks better
    private void ScaleIcons(GameObject icon, GameManager.CharacterChoice choice)
    {
        if (choice == GameManager.CharacterChoice.Tsuki)
        {
            icon.transform.localPosition = new Vector3(1.2f, 0f, 0f);
            icon.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
        }
        else
        {
            icon.transform.localPosition = new Vector3(0f, 0f, 0f);
            icon.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
        }
    }

    

}
