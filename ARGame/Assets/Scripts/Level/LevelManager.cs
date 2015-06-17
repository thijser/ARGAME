namespace Level
{
    using Network;
    using UnityEngine;
    using Vision;

    public class LevelManager : MonoBehaviour
    {
        public int currentLevelIndex = 0;
        public Vector2 boardsize { get; set; }
        public float IARscale { get; set; }

        private LevelLoader levelLoader = new LevelLoader();
        private GameObject level;

        public void Start()
        {
            IARscale = 1;
            IARLink link = gameObject.GetComponent<IARLink>();
            if (link != null)
            {
                IARscale = link.GetScale();
            }

            this.restartGame();
        }

        public void nextLevel()
        {
            loadLevel(++currentLevelIndex);
        }

        public void restartLevel()
        {
            loadLevel(currentLevelIndex);
        }

        public void restartGame()
        {
            this.loadLevel(0);
        }

        public void loadLevel(int index)
        {
            GameObject.Destroy(this.level);
            Debug.Log("loading level" + index);
            this.level = this.levelLoader.CreateLevel("Assets/resources/Levels/" + index + ".txt");
            this.currentLevelIndex = index;
            this.level.transform.SetParent(transform);
            this.scaleLevel();
        }

        public void OnLevelUpdate(LevelUpdate levelup)
        {
            boardsize = levelup.Size;
            if (currentLevelIndex != levelup.NextLevelIndex)
            {
                loadLevel(levelup.NextLevelIndex);
            }
        }

        public void scaleLevel()
        {
            Levelcomp levelcomp = level.GetComponent<Levelcomp>();
            float xproportions = boardsize.x / levelcomp.size.x;
            float yproportions = boardsize.y / levelcomp.size.y;
            if (xproportions < yproportions)
                level.transform.localScale = new Vector3(xproportions, xproportions, xproportions) * IARscale;
            else
                level.transform.localScale = new Vector3(yproportions, yproportions, yproportions) * IARscale;
        }

    }
}