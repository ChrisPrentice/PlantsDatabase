using System;

namespace PlantsDatabase
{
    public static class ImageSearchUriCreator
    {
        public static string Create(string plantType, string plantName)
        {
            string plantNameString = $"{plantType}+{plantName.Replace(" ", "+")}";

            return
                $"https://www.google.co.uk/search?hl=en&site=imghp&tbm=isch&source=hp&biw=1920&bih=911&q={plantNameString}&oq={plantNameString}&gs_l=img.3...21345.28807.0.29709.22.5.0.17.17.0.145.377.4j1.5.0....0...1ac.1.64.img..0.6.377...0.LXGAkqOlc5Q";
        }
    }
}