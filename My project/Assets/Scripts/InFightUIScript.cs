using UnityEngine;
using UnityEngine.UIElements;

public class InFightUIScript : MonoBehaviour
{
    private GameManager gameManager;
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    [Header("--------Chosen Map--------")]
    public GameManager.MapChoice map;
    void Awake()
    {
        gameManager = GameManager.Instance;
        spriteRenderer = GameObject.Find("Backround").GetComponent<SpriteRenderer>();
        ChooseMap();
    }

    // Update is called once per frame
    public void ChooseMap()
    {
        if (map == GameManager.MapChoice.Map1)
        {
            spriteRenderer.sprite = spriteArray[0];
        }
        else if (map == GameManager.MapChoice.Map2)
        {
            spriteRenderer.sprite = spriteArray[1];
        }

    }
    public void SetMap(int index)
    {
        if (index == 0)
        {
            map = GameManager.MapChoice.Map1;
        }
        else
        {
            map = GameManager.MapChoice.Map2;
        }
    }

}
