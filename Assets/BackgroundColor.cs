using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    public float every;   //The public variable "every" refers to "Lerp the color every X"
    float colorstep;
    public Color[] colors = new Color[5]; //Insert how many colors you want to lerp between here, hard coded to 4
    int i;
    Color lerpedColor = Color.red;  //This should optimally be the color you are going to begin with

    void Start()
    {

        //In here, set the array colors you are going to use, optimally, repeat the first color in the end to keep transitions smooth

        //colors[0] = Color.red;
        //colors[1] = Color.yellow;
        //colors[2] = Color.cyan;
        //colors[3] = Color.red;

    }


    // Update is called once per frame
    void Update()
    {

        if (colorstep < every)
        { //As long as the step is less than "every"
            lerpedColor = Color.Lerp(colors[i], colors[i + 1], colorstep);
            this.GetComponent<Camera>().backgroundColor = lerpedColor;
            colorstep += 0.001f;  //The lower this is, the smoother the transition, set it yourself
        }
        else
        { //Once the step equals the time we want to wait for the color, increment to lerp to the next color

            colorstep = 0;

            if (i < (colors.Length - 2))
            { //Keep incrementing until i + 1 equals the Lengh
                i++;
            }
            else
            { //and then reset to zero
                i = 0;
            }
        }
    }
}
