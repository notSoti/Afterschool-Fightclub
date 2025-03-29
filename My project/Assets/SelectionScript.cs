using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool char1;
    public bool char2;
    public bool char3;
    public bool char4;

    public void SelectChar1() {
        char1 = true;
        char2 = false;
        char3 = false;
        char4 = false;
    }

    public void SelectChar2()
    {
        char1 = false;
        char2 = true;
        char3 = false;
        char4 = false;
    }

    public void SelectChar3()
    {
        char1 = false;
        char2 = false;
        char3 = true;
        char4 = false;
    }

    public void SelectChar4()
    {
        char1 = false;
        char2 = false;
        char3 = false;
        char4 = true;
    }

}
