
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using YourPetAPI.Exceptions;

namespace PixelArtGeneratorBackend.Controllers
{
    [Route("api/pixel")]
    public class PixelController : ControllerBase
    {
        [HttpPost]
        public IActionResult GeneratePixelArt(IFormFile imageFile, int? width, int? height, PixelColorPalette colorPalette, int option)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return BadRequest("Nieprawidłowe dane wejściowe.");
                if (width > 5000 || height > 5000)
                    return BadRequest("Za duży rozmiar zdjęcia.");
                // Przekształć obraz na piksel art
                var pixelArt = GeneratePixelArtFromImage(imageFile.OpenReadStream(), width, height, colorPalette, option);

                // Zapisz pixel art do strumienia
                var memoryStream = new MemoryStream();
                pixelArt.Save(memoryStream, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
                memoryStream.Position = 0;

                // Wyślij odpowiedź z pixel artem
                return File(memoryStream, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private Image<Rgba32> GeneratePixelArtFromImage(Stream inputStream, int? width, int? height, PixelColorPalette colorPalette, int option)
        {
            // Przekształć obraz na piksel art
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream))
            {
                if (!width.HasValue && !height.HasValue)
                {
                    width = image.Width;
                    height = image.Height;
                }
                else if (!width.HasValue)
                {
                    width = (int)(image.Width * ((float)height / image.Height));
                }
                else if (!height.HasValue)
                {
                    height = (int)(image.Height * ((float)width / image.Width));
                }
                if(option == 1)
                {
                   image.Mutate(x => x.Resize(width.Value, height.Value).Pixelate());
                }
                else
                {

                    image.Mutate(x => x.Resize(width.Value, height.Value).Dither());
                }
               

                var pixelArt = new Image<Rgba32>(image.Width, image.Height);

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixelColor = image[x, y];

                        // Przypisz kolor piksela w pixel arcie na podstawie pixelColor
                        pixelArt[x, y] = GetClosestColor(pixelColor, colorPalette);
                    }
                }

                return pixelArt;
            }
        }

        private Rgba32 GetClosestColor(Rgba32 pixelColor, PixelColorPalette colorPalette)
        {
            if (colorPalette == PixelColorPalette.Unchanged)
                return pixelColor;

            var closestColor = colorPalette.Colors()[0];
            var minDistance = CalculateColorDistance(pixelColor, closestColor);

            foreach (var color in colorPalette.Colors())
            {
                var distance = CalculateColorDistance(pixelColor, color);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColor = color;
                }
            }

            return closestColor;
        }

        private float CalculateColorDistance(Rgba32 color1, Rgba32 color2)
        {
            var rDiff = color1.R - color2.R;
            var gDiff = color1.G - color2.G;
            var bDiff = color1.B - color2.B;

            return (rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff);
        }
    }

    public enum PixelColorPalette
    {
        Grayscale,
        Sepia,
        Neon,
        Unchanged,
        Winter,
        Muted
    }

    public static class PixelColorPaletteExtensions
    {
        public static List<Rgba32> Colors(this PixelColorPalette colorPalette)
        {
            switch (colorPalette)
            {
                case PixelColorPalette.Grayscale:
                    return new List<Rgba32>
                    {
                        new Rgba32(0, 0, 0),
                        new Rgba32(255, 255, 255)
                    };

                case PixelColorPalette.Sepia:
                    return new List<Rgba32>
                    {
                        new Rgba32(112, 66, 20),
                        new Rgba32(219, 173, 109)
                    };

                case PixelColorPalette.Neon:
                    return new List<Rgba32>
                    {
                        new Rgba32(255, 0, 0),
                        new Rgba32(0, 255, 0),
                        new Rgba32(0, 0, 255),
                        new Rgba32(255, 255, 0)
                    };

                case PixelColorPalette.Unchanged:
                    return new List<Rgba32>(); // Zwraca pustą listę, aby zachować niezmienione kolory

                // Dodatkowe palety kolorów
                case PixelColorPalette.Winter:
                    return new List<Rgba32>
                    {
                        new Rgba32(255, 255, 255),
                        new Rgba32(0, 0, 255),
                        new Rgba32(128, 128, 255),
                        new Rgba32(192, 192, 255),
                        new Rgba32(0, 128, 192)
                    };

                case PixelColorPalette.Muted:
                    return new List<Rgba32>
                    {
                        new Rgba32(128, 128, 128),
                        new Rgba32(192, 192, 192),
                        new Rgba32(224, 224, 224)
                    };

                default:
                    throw new CustomException("Nieobsługiwana paleta kolorów.");
            }
        }
    }
}

