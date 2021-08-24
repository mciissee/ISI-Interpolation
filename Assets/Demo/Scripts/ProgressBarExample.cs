/************************************************************************************************************************************
* Developed by Mamadou Cisse                                                                                                        *
* Mail => mciissee@gmail.com                                                                                                        *
* Twitter => http://www.twitter.com/IncMce                                                                                          *
* Unity Asset Store catalog: http://u3d.as/riS                                                                                      *
*************************************************************************************************************************************/


using UnityEngine;
using UnityEngine.UI;
using InfinityEngine;

public class ProgressBarExample : MonoBehaviour {

    public Text label;
    public Image progressBar;

    void Start()
    {
        Infinity.To(0.0f, 1.0f, 2, value => {
            label.text = (int)(value * 100) + "%";
            progressBar.fillAmount = value;

            if(value >= (75 / 100) * 2.0 && progressBar.color != Color.red)
            {
                progressBar.color = Color.red;
            }
        }).Start();
    }

}