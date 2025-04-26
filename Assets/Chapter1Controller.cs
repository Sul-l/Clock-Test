using System.Collections;
using UnityEngine;

public class Chapter1Controller : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(ProceedAfterDelay());
    }

    IEnumerator ProceedAfterDelay()
    {
        yield return new WaitForSeconds(7f);

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            int nextSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
            gm.ChangeMainScene(nextSceneIndex); // Go to the train cutscene
        }
    }
}
