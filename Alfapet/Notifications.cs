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
            for(var i = 0; i < Messages.Count; i++)
            {
                // 2 sekunder har gått sedan medelandet var tillagt
                if (DateTimeOffset.Now.ToUnixTimeSeconds() - Messages[i].StartTime > 2)
                {
                    // Lerpar positionen till utanför skärmen
                    H = MathHelper.Lerp(H, -20, (float)(DateTimeOffset.Now.ToUnixTimeSeconds() - Messages[i].StartTime + 2) / 500);
                    if (H <= -14) // Texten är utanför (14 är storleken på font)
                    {
                        Messages.Remove(Messages[i]);

                        H += 100; // För att nästa meddelande hamnar på start positionen
                        return;
                    }
                }
                
                Ui.DrawCenterText(Ui.MontserratBoldTiny, Messages[i].Value, new Vector2(0, 0),
                    new Vector2(Alfapet.Graphics.GraphicsDevice.Viewport.Width, H + i * 100), Color.White);
            }
        }
    }
}