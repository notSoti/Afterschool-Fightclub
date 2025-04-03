using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public enum Character{Tsuki, Jasmine, Mihu, Kagkur};
    public Character player = Character.Tsuki; //default option

    public void SelectChar1() {
        Character player = Character.Tsuki;
    }

    public void SelectChar2() {
        Character player = Character.Jasmine;
    }

    public void SelectChar3() {
        Character player = Character.Mihu;
    }

    public void SelectChar4() {
        Character player = Character.Kagkur;
    }

    public Character GetP1() {
        return player;
    }
    public void SetP1(Character newcharacter) {
        player = newcharacter;
    }

}
