using UnityEngine;

namespace Assets.Scripts.Scenes
{
    public class SceneManager : MonoBehaviour
    {
        public void OpenScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}