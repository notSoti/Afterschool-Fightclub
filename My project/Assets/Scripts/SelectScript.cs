using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public enum Character { Tsuki, Mihu, Jasmine };
    public Character player;

    void Start() {
        player = (Character)Random.Range(0, 1); //default option
    }

    void SelectChar1() {
        player = Character.Tsuki;
    }

    void SelectChar2() {
        player = Character.Jasmine;
    }

    void SelectChar3() {
        player = Character.Mihu;
    }

    void SelectChar4() {
        player = (Character)Random.Range(0, 1);
    }

    public Character GetP1() {
        return player;
    }
    public void SetP1(Character newcharacter) {
        player = newcharacter;
    }

}
