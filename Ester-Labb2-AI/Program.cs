using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        
        string endpoint = "https://aiestervision.cognitiveservices.azure.com/";
        string key = "3199829af13d42abb88d555c1b9c8361";

        // Skapa klient för Computer Vision
        var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endpoint
        };

        // Be användaren om en bildsökväg eller URL
        Console.WriteLine("Ange URL till bilden:");
        string imageUrl = Console.ReadLine();

        // Kontrollera att URL:en är giltig
        if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
        {
            Console.WriteLine("Ogiltig URL. Kontrollera URL:en.");
            return;
        }

        try
        {
            // Analysera bilden
            var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Tags, VisualFeatureTypes.Objects };
            var analysis = await client.AnalyzeImageAsync(imageUrl, features);

            // Visa analysresultat
            foreach (var tag in analysis.Tags)
            {
                Console.WriteLine($"Tag: {tag.Name}, Confidence: {tag.Confidence}");
            }

            foreach (var obj in analysis.Objects)
            {
                Console.WriteLine($"Object: {obj.ObjectProperty}, Bounding box: {obj.Rectangle}");
            }

            // Be användaren om storleken på miniatyrbilden
            Console.WriteLine("Ange bredden på miniatyrbilden:");
            int width = int.Parse(Console.ReadLine());
            Console.WriteLine("Ange höjden på miniatyrbilden:");
            int height = int.Parse(Console.ReadLine());

            // Skapa miniatyrbild
            var thumbnailStream = await client.GenerateThumbnailAsync(width, height, imageUrl, true);
            using var fileStream = new FileStream("thumbnail.jpg", FileMode.Create, FileAccess.Write);
            await thumbnailStream.CopyToAsync(fileStream);

            Console.WriteLine("Miniatyrbild skapad och sparad som thumbnail.jpg");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel uppstod: {ex.Message}");
        }
    }
}
