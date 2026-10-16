using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary
{
    public class Timer
    {
        public float Duration { get; set; }
        public float RemainingTime { get; set; }

        public bool IsRunning => RemainingTime > 0f;
        public bool IsFinished => RemainingTime <= 0f;

        public Timer(float duration)
        {
            Duration = duration;
            RemainingTime = 0f;
        }
        public void Start()
        {
            RemainingTime = Duration;
        }
        public void Stop()
        {
            RemainingTime = 0f;
        }  
        public void Update(GameTime gameTime)
        {
            if (!IsRunning)
            {
                return;
            }

            RemainingTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (RemainingTime < 0f)
            {
                    RemainingTime = 0f;
            }
        }
    }
}
