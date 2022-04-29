using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public HubBase hub; // This is the LEGO Hub. Here you can set the LED Color and check the connection status.
    Motor motor; // A regular motor. You can set the power to make it spin. Between -100 and 100. To stop set the power to 0.
    TachoMotor tachoMotor; // Tacho motors can detect their rotational position, and use that to calculate a Position and Speed value. All TachoMotors are Motors, but not vice versa.
    DistanceSensor distanceSensor; // Distance sensors emit ultrasonic sound and listen for the ecco to detect distance to surfaces in front of them. 
    ColorSensorTechnic colorSensor; // Color sensor emit white light, and look for that light's bounce color, to determine the color of objects right in front of them.
    ForceSensor forceSensor; // A force sensor is similar to an analogue trigger button, with a "Force" value between 0 and 1 depending on how far it's pressed.
    new WhiteLight light; // White Light is a pair of white LED lights, that you can set the brightness of, between 0 and 100.


    public TMP_Text motorPositionText, motorSpeedText, distanceText, forceText, motorControls, tachoControls, lightControls, distanceControls;
    public Image colorOut;
    public int position = 0;
    public int speed = 0;
    public float distance = 0;
    public int force = 0;
    public Color colorRead;

    void Start()
    {
            playTips.SetActive(true);
            editorTips1.SetActive(false);
            editorTips2.SetActive(false);

        foreach (ServiceBase service in hub.externalServices) //Automatically setting references, to what has been defined as External Services on the hub
        {
            if (service is TachoMotor)
            {
                tachoMotor = (TachoMotor)service;
            }
            else if (service is Motor)
            {
                motor = (Motor)service;
            }
            else if (service is DistanceSensor)
            {
                distanceSensor = (DistanceSensor)service;
            }
            else if (service is ColorSensorTechnic)
            {
                colorSensor = (ColorSensorTechnic)service;
            }
            else if (service is ForceSensor)
            {
                forceSensor = (ForceSensor)service;
            }
            else if (service is WhiteLight)
            {
                light = (WhiteLight)service;
            }
        }
    }

    // Change the color of the HUB's LED to a Random color.
    public void SetRandomLEDColor()
    {
        hub.LedColor = new Color(Random.value, Random.value, Random.value);
    }

    public void OnHubConnected(bool connected)
    {
        if (connected)
        {
            debugOut.SetActive(true);
            playTips.SetActive(false);
            editorTips1.SetActive(false);
            editorTips2.SetActive(false);
        }
    }

    void Update()
    {
        if (motor != null && motor.IsConnected)
            UpdateMotor();

        if (tachoMotor != null && tachoMotor.IsConnected)
            UpdateTachoMotor();

        if (distanceSensor != null && distanceSensor.IsConnected)
            UpdateDistanceSensor();

        if (colorSensor != null && colorSensor.IsConnected)
            UpdateColorSensor();

        if (forceSensor != null && forceSensor.IsConnected)
            UpdateForceSensor();

        if (light != null && light.IsConnected)
            UpdateLight();
    }

    public GameObject playTips, editorTips1, editorTips2, debugOut;

    public void ToggleTips()
    {
        editorTips1.SetActive(debugOut.activeSelf);
        editorTips2.SetActive(editorTips1.activeSelf);
        debugOut.SetActive(!editorTips1.activeSelf);
    }

    void UpdateMotor()
    {
        motorControls.text = "Hold Up or Down to spin the Motor";

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            motor.SetPower(100); // Turn the motor Clockwise at full speed.
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            motor.SetPower(-100); // Turn the motor Counter-Clockwise at full speed.
        }
        else if ((Input.GetKeyUp(KeyCode.DownArrow)) || (Input.GetKeyUp(KeyCode.UpArrow)))
        {
            motor.SetPower(0); // Stop the motor.
        }
    }

    void UpdateTachoMotor()
    {
        tachoControls.text = "Hold Up or Down to spin the Motor\nPress \"S\" to Spin for 1 second\nPress \"G\" to go to the zero position";

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            tachoMotor.SetPower(100); // Turn the motor Clockwise at full speed.
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            tachoMotor.SetPower(-100); // Turn the motor Counter-Clockwise at full speed.
        }
        else if ((Input.GetKeyUp(KeyCode.DownArrow)) || (Input.GetKeyUp(KeyCode.UpArrow)))
        {
            tachoMotor.SetPower(0); // Stop the motor.
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Go To Position");
            tachoMotor.GoToPosition(0, true, 50); // Move the motor at half speed to the zero-position.
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Spin For Time");
            tachoMotor.SpinForTime(1000, 50, false); // Move the motor at half speed Clockwise for 1000 milliseconds (1 second).
        }

        position = tachoMotor.Position; // Read the current position of the tacho motor.
        speed = tachoMotor.Speed; // Read the current speed of the tacho motor.

        if (motorPositionText)
            motorPositionText.text = "Motor Position: " + position;

        if (motorSpeedText)
            motorSpeedText.text = "Motor Speed: " + speed;
    }

    bool upperLeftLightOn, upperRightLightOn, lowerLeftLightOn, lowerRightLightOn;

    void UpdateDistanceSensor()
    {
        distanceControls.text = "Press \"1\"-\"4\" to toggle lights in distance sensor";

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            upperLeftLightOn = !upperLeftLightOn;
            distanceSensor.SetIntensity(upperLeftLightOn ? 100 : 0, upperRightLightOn ? 100 : 0, lowerLeftLightOn ? 100 : 0, lowerRightLightOn ? 100 : 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            upperRightLightOn = !upperRightLightOn;
            distanceSensor.SetIntensity(upperLeftLightOn ? 100 : 0, upperRightLightOn ? 100 : 0, lowerLeftLightOn ? 100 : 0, lowerRightLightOn ? 100 : 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            lowerLeftLightOn = !lowerLeftLightOn;
            distanceSensor.SetIntensity(upperLeftLightOn ? 100 : 0, upperRightLightOn ? 100 : 0, lowerLeftLightOn ? 100 : 0, lowerRightLightOn ? 100 : 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            lowerRightLightOn = !lowerRightLightOn;
            distanceSensor.SetIntensity(upperLeftLightOn ? 100 : 0, upperRightLightOn ? 100 : 0, lowerLeftLightOn ? 100 : 0, lowerRightLightOn ? 100 : 0);
        }

        distance = distanceSensor.Distance; // Read the current speed of the distance sensor.
        if (distanceText)
            distanceText.text = "Distance: " + distance;
    }

    private void UpdateColorSensor()
    {
        colorRead = colorSensor.Color; // Read the current value of the color sensor.
        if (colorOut)
            colorOut.color = colorRead;
    }

    bool forceReset = false;

    void UpdateForceSensor()
    {
        force = forceSensor.Force; // Read the current value of the force sensor.
        if (forceText != null)
            forceText.text = "Force: " + force;

        if (force == 0 && !forceReset)
            forceReset = true;
        if (force >= 50 && forceReset) // If the force sensor is pressed beyond halfway, change the HUB's LED light color.
        {
            forceReset = false;
            SetRandomLEDColor();
        }
    }

    #region WhiteLights
    bool animatingLight = false;

    //Control the lights
    private void UpdateLight()
    {
        lightControls.text = "Press \"L\" to fade in the lights";

        if (!animatingLight && Input.GetKeyDown(KeyCode.L))
            StartCoroutine(LightAnim()); // Start controlling the light when you press the "L"-key.
    }

    IEnumerator LightAnim()
    {
        animatingLight = true;
        float t;
        float duration = 2.5f;
        t = Time.deltaTime;
        while (t > 0)
        {
            if (Input.GetKey(KeyCode.L)) // While you hold down the "L" key, the light will get brighter.
                t += Time.deltaTime;
            else // If you let go, the light will dim.
                t -= Time.deltaTime;

            if (t > duration)
                t = duration;

            light.SetIntensity((int)((t / duration) * 100));
            yield return null;
        }

        light.SetIntensity(0);
        animatingLight = false; // Stop the coroutine.
    }
    #endregion
}
