namespace Core {
    using UnityEngine;

    public class WinScreen : MonoBehaviour {
        public readonly float TransitionTime = 5;

        public TextMesh MainText { get; private set; }
        public TextMesh SubText { get; private set; }
        public Camera OrthoCamera { get; private set; }

        public bool ShowingWinScreen
        {
            get { return this.OrthoCamera.enabled; }
        }

        public void Start() {
            this.MainText = transform.Find("MainText").GetComponent<TextMesh>();
            this.SubText = transform.Find("SubText").GetComponent<TextMesh>();

            this.OrthoCamera = transform.Find("Camera").GetComponent<Camera>();

            // Initially don't draw the win screen
            this.OrthoCamera.enabled = false;
        }

        public void FinishLevel(int timeSpent, int nextLevel) {

        }

        public void Update() {
            
        }
    }
}