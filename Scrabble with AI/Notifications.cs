using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Alfapet
{
    public class Notifications : Game
    {
        private static readonly List<Message> Messages = new List<Message>();
        private static float H = 100;
        private struct Message
        {
            public string Value { get; set; }
            public long StartTime { get; set; }
        }

        public static void AddMessage(string msg)
        {
            Messages.Add(new Message
            {
                Value = msg,
                StartTime = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
        }

        public static void Draw()
        {
            for (var i = 0; i < Messages.Count; i++)
            {
                // 2 seconds have passed since the message was added
                if (DateTimeOffset.Now.ToUnixTimeSeconds() - Messages[i].StartTime > 2)
                {
                    // Lerps the position to outside of the screen
                    H = MathHelper.Lerp(H, -20, (float)(DateTimeOffset.Now.ToUnixTimeSeconds() - Messages[i].StartTime + 2) / 100);
                    if (H <= -14) // The text is off-screen (14 is the font size)
                    {
                        Messages.Remove(Messages[i]);

                        H += 100; // To position the next message at the start position
                        return;
                    }
                }

                Ui.DrawCenterText(Ui.MontserratBoldTiny, Messages[i].Value, new Vector2(0, 0),
                    new Vector2(Alfapet.Graphics.GraphicsDevice.Viewport.Width, H + i * 100), Color.White);
            }
        }
    }
}
