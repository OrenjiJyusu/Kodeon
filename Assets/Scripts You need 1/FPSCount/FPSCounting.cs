using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AP3X {
    public class FPSCounter : MonoBehaviour
    {
        public float updateInterval = 1.0f;
        public TextMeshProUGUI fpsText;
        public bool ImAGeek = false;
        public TextMeshProUGUI avgFpsText;
        public TextMeshProUGUI minFpsText;
        public TextMeshProUGUI maxFpsText;
        public Color lowFpsColor = Color.red;
        public Color mediumFpsColor = Color.yellow;
        public Color highFpsColor = Color.green;
        public int lowFpsThreshold = 30;
        public int mediumFpsThreshold = 60;

        private float accumulator = 0.0f;
        private int frames = 0;
        private float timeLeft;
        private int framerate;
        private int minFramerate = int.MaxValue;
        private int maxFramerate = int.MinValue;
        private int avgFramerate;

        void Start()
        {
            timeLeft = updateInterval;
            ToggleDetailedStats();
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;
            accumulator += Time.timeScale / Time.deltaTime;
            frames++;

            if (timeLeft <= 0.0f)
            {
                framerate = Mathf.FloorToInt(accumulator / frames);
                fpsText.text = $"FPS: {framerate}";

                if (ImAGeek)
                {
                    minFramerate = Mathf.Min(minFramerate, framerate);
                    maxFramerate = Mathf.Max(maxFramerate, framerate);
                    avgFramerate = (int)(accumulator / frames);

                    avgFpsText.text = $"Avg FPS: {avgFramerate}";
                    minFpsText.text = $"Min FPS: {minFramerate}";
                    maxFpsText.text = $"Max FPS: {maxFramerate}";
                }

                fpsText.color = GetFpsColor(framerate);

                accumulator = 0.0f;
                frames = 0;
                timeLeft = updateInterval;
            }
        }

        public void ToggleImAGeek()
        {
            ImAGeek = !ImAGeek;
            ToggleDetailedStats();
        }

        private void ToggleDetailedStats()
        {
            avgFpsText.enabled = ImAGeek;
            minFpsText.enabled = ImAGeek;
            maxFpsText.enabled = ImAGeek;
        }

        Color GetFpsColor(int fps)
        {
            if (fps < lowFpsThreshold)
                return lowFpsColor;
            else if (fps < mediumFpsThreshold)
                return mediumFpsColor;
            else
                return highFpsColor;
        }
    }
}
