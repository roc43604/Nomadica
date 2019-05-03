using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    public class Hue
    {
        public const float DayStart = (float)(Math.PI) / 8f,
                           DayEnd = (float)(Math.PI) / 8f * 7f,
                           DayLength = DayEnd - DayStart; 

        public const float DuskStart = DayEnd,
                           DuskEnd = (float)(Math.PI) / 8f * 9f,
                           DuskLength = DuskEnd - DuskStart; 

        public const float NightStart = DuskEnd,
                           NightEnd = (float)(Math.PI) / 8f * 15f,
                           NightLength = NightEnd - NightStart;

        public const float DawnStart = NightEnd,
                           DawnEndWrapped = DayStart,
                           DawnEndContinued = (float)(Math.PI) / 8 * 17f,
                           DawnLength = (float)(Math.PI) / 4f;                

        public const float YellowRangeStart = (float)(Math.PI) / 8f,
                           YellowRangleEnd = (float)(Math.PI) / 8f * 7f;
        public static readonly Color DuskDawnColor = Color.Yellow; 

        public const float NightRangeStart = (float)(Math.PI),
                           NightRangeEnd = (float)(Math.PI) * 2f;
        public static readonly Color NightColor = Color.Blue;

        /// <summary>
        /// The color this hue is (refer to readonly's above). It will
        /// be yellow if dusk/dawn, and blue if night. 
        /// </summary>
        public Color Color;

        /// <summary>
        /// The type of hue showing, whether its day, dawn/dusk, night, 
        /// or something else entirely. 
        /// </summary>
        public WeatherAndTime Status;

        /// <summary>
        /// A number 0-1 that describes the intensity of the hue. 
        /// </summary>
        public float Amount;

        /// <summary>
        /// Whether or not the sun is giving off light.  
        /// </summary>
        public bool SunlightVisible; 

        public Hue(Color Color, WeatherAndTime Status, float Amount, bool SunlightVisible)
        {
            this.Color = Color;
            this.Status = Status;
            this.Amount = Amount;
            this.SunlightVisible = SunlightVisible; 
        }
    }
}
