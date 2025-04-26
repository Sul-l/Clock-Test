using UnityEngine;
using UnityEngine.UI;

public class ClockPuzzle : MonoBehaviour
{
    [Header("Hand References")]
    public Transform hourHand;
    public Transform minuteHand;

    [Header("UI Buttons")]
    public Button rotateHourButton;
    public Button rotateMinuteButton;

    [Header("Rotation Settings")]
    public float hourRotationStep = 30f;    // 360 / 12
    public float minuteRotationStep = 180f; // 360 / 2

    [Header("Correct Time (Set Here)")]
    public int correctHourIndex = 7;  // 7:30
    public int correctMinuteIndex = 1;

    [Header("Optional")]
    public AudioSource puzzleSolvedSound;

    private int hourIndex = 0;   // 0 to 11
    private int minuteIndex = 0; // 0 or 1
    private bool puzzleSolved = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (rotateHourButton != null)
            rotateHourButton.onClick.AddListener(RotateHourHand);
        if (rotateMinuteButton != null)
            rotateMinuteButton.onClick.AddListener(RotateMinuteHand);
    }

    void RotateHourHand()
    {
        if (puzzleSolved) return;

        hourIndex = (hourIndex + 1) % 12;
        hourHand.localEulerAngles = new Vector3(0, 0, -hourRotationStep * hourIndex);
        CheckIfSolved();
    }

    void RotateMinuteHand()
    {
        if (puzzleSolved) return;

        minuteIndex = (minuteIndex + 1) % 2;
        minuteHand.localEulerAngles = new Vector3(0, 0, -minuteRotationStep * minuteIndex);
        CheckIfSolved();
    }

    void CheckIfSolved()
    {
        if (hourIndex == correctHourIndex && minuteIndex == correctMinuteIndex)
        {
            puzzleSolved = true;

            if (puzzleSolvedSound != null)
                puzzleSolvedSound.Play();

            StartCoroutine(TransitionToNextScene());
        }
    }

    private System.Collections.IEnumerator TransitionToNextScene()
    {
        // wait a moment for the player to hear the sound
        yield return new WaitForSeconds(1.5f);

        if (gameManager != null)
        {
            // Local scene 6
            gameManager.StartCoroutine(gameManager.ChangeScene(6, 0));
        }
        else
        {
            Debug.LogError("GameManager not found for scene transition.");
        }
    }
}
