using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CrystalTracker : MonoBehaviour
{
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public Text text1;
    public Text text2;
    public Text text3;
    public Slider slider;
    public Text crystalText;
    public int stars = 0;
    public Color yellow;
    public Color brown;

    public LevelController levelController;
    // Start is called before the first frame update
    void Awake()
    {
        slider.maxValue = levelController.reqCrystal1;
        slider.value = 0;

        text1.text = levelController.reqCrystal1.ToString();
        text2.text = levelController.reqCrystal2.ToString();
        text3.text = levelController.reqCrystal3.ToString();
        //yellow = new Color(255f / 255f, 233f / 233f, 0f / 255f);
        //brown = new Color(114f / 103f, 88f / 233f, 0f / 255f);


        if (levelController.reqCrystal1 == 0)
        {
            foreach (GameObject winBox in GameObject.FindGameObjectsWithTag("WinBox"))
            {
                winBox.GetComponent<GoodCube>().active = true;
            }
            stars = 1;
            star1.GetComponent<Image>().color = yellow;
            text1.color = brown;
            slider.maxValue = levelController.reqCrystal2;
        }
        if (levelController.reqCrystal2 == 0)
        {
            stars = 2;
            star2.GetComponent<Image>().color = yellow;
            text2.color = brown;
            slider.maxValue = levelController.reqCrystal3;
        }
        if (levelController.reqCrystal3 == 0)
        {
            stars = 3;
            star3.GetComponent<Image>().color = yellow;
            text3.color = brown;
            slider.maxValue = 1;
            slider.normalizedValue = 1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, levelController.crystals, Time.deltaTime / 0.2f);

        //if (levelController.reqCrystal3 == 0)
        //{
        //    slider.normalizedValue = 1;
        //}
    }

    public void AddCrystal(int count)
    {
        int newCount = levelController.crystals + count;
        if (newCount >= levelController.reqCrystal1 && stars < 1)
        {
            foreach (GameObject winBox in GameObject.FindGameObjectsWithTag("WinBox"))
            {
                winBox.GetComponent<GoodCube>().active = true;
            }
            star1.GetComponent<Image>().color = Color.yellow;
            text1.color = brown;

            star1.GetComponent<Animator>().SetTrigger("Boop");
            //star1.GetComponent<Animator>().ResetTrigger("Boop");
            stars = 1;

            
            slider.maxValue = levelController.reqCrystal2;
            slider.normalizedValue = 1;

        }
        if (newCount >= levelController.reqCrystal2 && stars < 2)
        {
            star2.GetComponent<Image>().color = yellow;
            text2.color = brown;
            star2.GetComponent<Animator>().SetTrigger("Boop");
            //star2.GetComponent<Animator>().ResetTrigger("Boop");
            stars = 2;

            
            slider.maxValue = levelController.reqCrystal3;
            slider.normalizedValue = 1;
        }
        if (newCount >= levelController.reqCrystal3 && stars < 3)
        {
            star3.GetComponent<Image>().color = yellow;
            text3.color = brown;
            star3.GetComponent<Animator>().SetTrigger("Boop");
            //star3.GetComponent<Animator>().ResetTrigger("Boop");
            stars = 3;
            slider.value = levelController.crystals;
        }
        
        levelController.crystals += count;
        crystalText.text = levelController.crystals.ToString();
        crystalText.gameObject.GetComponent<Animator>().SetTrigger("Boop");

    }

    

    
}
