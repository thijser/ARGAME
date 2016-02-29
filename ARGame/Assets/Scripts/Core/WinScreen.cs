namespace Core {
    using UnityEngine;

    public class WinScreen : MonoBehaviour {
        public readonly float TransitionTime = 5;

        private TextMesh MainText;
        private TextMesh SubText;
        private Camera OrthoCamera;

        private bool ShowingWinScreen;
        private float ;
        private int 

        public void Start() {
            MainText = transform.Find("MainText").GetComponent<TextMesh>();
            SubText = transform.Find("SubText").GetComponent<TextMesh>();

            OrthoCamera = transform.Find("Camera").GetComponent<Camera>();

            // Initially don't draw the win screen
            OrthoCamera.enabled = false;
        }

        public void FinishLevel(int timeSpent, int nextLevel) {

        }

        public void Update() {
            
        }
    }
}