using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FireworkManager : MonoBehaviour
{
    //Fields
    //Public
    public int numFireworks = 3;
    public GameObject fireworkPrefab;
    public ParticleSystem explosion;
    public float timeBetweenLaunches = 10.0f;
    public Vector2 launchVelocity;
    public Text colorText;
    public AudioClip cheeringAudio;
    public AudioClip booingAudio;
    public AudioClip explosionAudio;
    public AudioClip launchAudio;
    
    //Private
    private List<Firework> fireworks;
    private bool fireworksLaunched = false;
    private bool clickedRightFirework = false;
    private bool alreadyExploded = false;
    private bool alreadyCalled = false;
    private float launchTimer = 0;
    private FireworkColors nextColorCalled;
    private FireworkColors lastColorCalled;
    private string cheering = "YAY!!!";
    private string booing = "BOO!!!";
    private AudioSource Audio;
    
    // Use this for initialization
    void Start()
    {
        Audio = GetComponent<AudioSource>();
      
        Vector2 almostTopLeft = GameObject.Find("Main Camera").GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 1.0f));
        Vector2 almostBottomLeft = GameObject.Find("Main Camera").GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, .3f));

        fireworks = new List<Firework>();

        for(int i = 0; i < numFireworks; i++)
        {
            GameObject obj = Instantiate(fireworkPrefab);
            obj.transform.position = transform.position;
            obj.transform.parent = transform;
            fireworks.Add(obj.GetComponent<Firework>());
            fireworks[i].manager = this;
            fireworks[i].explosionSystem = explosion;
            fireworks[i].launchPoint = new Vector2(almostTopLeft.x - fireworkPrefab.transform.localScale.x * 2, Mathf.Lerp(almostTopLeft.y, almostBottomLeft.y, ((float)i + 1) / numFireworks));
        }
        launchTimer = 2;
    }

    // Update is called once per frame
    void Update()
    {
        launchTimer -= Time.deltaTime;
        if(launchTimer <= 0 && alreadyCalled == false)
        {
            alreadyCalled = true;
            nextColorCalled = GetRandomFireworkColor();
            //RUCHITA - Trigger the audience yelling the color name here. Use nextColorCalled as the color that they yell out.
            //There is a method called GetStringFromEnumValue() below that will return the string of the color.
            //Just call it like: GetStringFromEnumValue(nextColorCalled), and use the string it returns to have the crowd shout.
            //There is also a method called GetColorFromEnumValue() which you can also pass nextColorCalled to get the Color object back
            //to use when you have the text draw. If you have any questions just message me @haroldthehobo in the discord channel.
            colorText.GetComponent<Text>().text = GetStringFromEnumValue(nextColorCalled);
            colorText.GetComponent<Text>().color = GetColorFromEnumValue(nextColorCalled);
            Debug.Log("We want " + GetStringFromEnumValue(nextColorCalled) + "!");
        }
        if (launchTimer <= -3.0f && fireworksLaunched == false)
        {
            LaunchFireworkSet(nextColorCalled);
            launchTimer = timeBetweenLaunches;
        }
    }

    public void LaunchFireworkSet(FireworkColors neededColor)
    {
        lastColorCalled = neededColor;
        alreadyExploded = false;
        alreadyCalled = false;
        if (!CheckIfFireworksFlying())
        {
            var values = System.Enum.GetValues(typeof(FireworkColors));
            List<FireworkColors> colors = values.Cast<FireworkColors>().Where(val => val != neededColor).ToList();
            int targetIndex = Random.Range(0, fireworks.Count);
            for(int i = 0; i < fireworks.Count; i++)
            {
                if(i == targetIndex)
                {
                    fireworks[i].Launch(launchVelocity, true);
                    fireworks[i].gameObject.GetComponent<SpriteRenderer>().color = GetColorFromEnumValue(neededColor);
                   Audio.PlayOneShot(launchAudio);
                }
                else
                {
                    FireworkColors unique = colors[Random.Range(0, colors.Count)];
                    colors.Remove(unique);
                    fireworks[i].Launch(launchVelocity, false);
                    fireworks[i].gameObject.GetComponent<SpriteRenderer>().color = GetColorFromEnumValue(unique);
                   // Audio.PlayOneShot(launchAudio);
                }
            }
        }
    }

    public bool HandleExplosion(Firework fw)
    {
        if (!alreadyExploded)
        {
            explosion.transform.position = fw.transform.position;
            explosion.startColor = fw.gameObject.GetComponent<SpriteRenderer>().color;
            explosion.Play();
            Audio.PlayOneShot(explosionAudio);
            alreadyExploded = true;

            if(explosion.startColor == GetColorFromEnumValue(lastColorCalled))
            {
                //RUCHITA - Handle crowd cheering here, player clicked the right color
                colorText.GetComponent<Text>().text = cheering;
                Audio.PlayOneShot(cheeringAudio);
                Debug.Log("YAY!!!");
            }
            else
            {
                //RUCHITA - Handle crowd booing here, player clicked the wrong color
               colorText.GetComponent<Text>().text = booing;
               Audio.PlayOneShot(booingAudio);
               Debug.Log("BOO!!!");
            }

            return true;
        }
        else
        {
            return false;
        }
    }


    //HELPER FUNCTIONS
    public bool CheckIfFireworksFlying()
    {
        bool launched = false;
        foreach (Firework work in fireworks) //Ensure all the fireworks have returned.
        {
            if (work.IsFlying) { launched = true;}
        }
        fireworksLaunched = launched;
        return fireworksLaunched;
    }

    public void CheckForNoExplosion()
    {
        if(fireworksLaunched && !alreadyExploded)
        {
            //RUCHITA - Handle crowd booing here, player didn't click a firework
            colorText.GetComponent<Text>().text = booing;
            Audio.PlayOneShot(booingAudio);
            Debug.Log("BOO!!!");
            alreadyExploded = true;
        }
    }

    public Color GetColorFromEnumValue(FireworkColors col)
    {
        switch (col)
        {
            case FireworkColors.Red:
                return Color.red;
            case FireworkColors.Green:
                return Color.green;
            case FireworkColors.Blue:
                return Color.cyan;
            case FireworkColors.Purple:
                return Color.magenta;
            case FireworkColors.White:
                return Color.white;
            case FireworkColors.Yellow:
                return Color.yellow;
            default:
                return Color.black;
        }
    }

    public string GetStringFromEnumValue(FireworkColors col)
    {
        switch (col)
        {
            case FireworkColors.Red:
                return "RED";
            case FireworkColors.Green:
                return "GREEN";
            case FireworkColors.Blue:
                return "BLUE";
            case FireworkColors.Purple:
                return "PURPLE";
            case FireworkColors.White:
                return "WHITE";
            case FireworkColors.Yellow:
                return "YELLOW";
            default:
                return "BLACK";
        }
    }

    public FireworkColors GetRandomFireworkColor()
    {
        var values = System.Enum.GetValues(typeof(FireworkColors));
        return (FireworkColors)values.GetValue(Random.Range(0, values.Length));
    }
}

public enum FireworkColors
{
    Red,
    Green,
    Blue,
    Purple,
    White,
    Yellow
};
