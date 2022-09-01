using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace Lab2_AI_ML
{
    class Program
    {
        static CustomVisionPredictionClient prediction_client;
        static void Main(string[] args)
        {
            MainMenu();

            void MainMenu()
            {
                Console.Clear();

                try
                {
                    IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                    IConfigurationRoot configuration = builder.Build();
                    string prediction_endpoint = configuration["PredictionEndpoint"];
                    string prediction_key = configuration["PredictionKey"];
                    Guid project_id = Guid.Parse(configuration["ProjectID"]);
                    string model_name = configuration["ModelName"];

                    prediction_client = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(prediction_key))
                    {
                        Endpoint = prediction_endpoint
                    };

                    try
                    {
                        Console.WriteLine("Cat/Dog Classifier \n-------------------- \n1. Enter local file path\n2. Enter image url\n");
                        var userInput = Convert.ToInt32(Console.ReadLine());

                        if(userInput == 1)
                        {
                            Console.Clear();

                            Console.Write("Enter local file path: ");
                            try
                            {
                                var userInput_ = Console.ReadLine();

                                MemoryStream image_data = new MemoryStream(File.ReadAllBytes(userInput_));
                                var result = prediction_client.ClassifyImage(project_id, model_name, image_data);

                                foreach (var item in result.Predictions)
                                {
                                    Console.WriteLine($"{item.TagName} {item.Probability:P1}");
                                }
                                Console.WriteLine("\npress any key to return to main menu");
                                Console.ReadKey();
                                MainMenu();
                            }
                            catch
                            {
                                Console.WriteLine("Error, press any key to return to main menu");
                                Console.ReadKey();
                                MainMenu();
                            }  
                        }
                        else if(userInput == 2)
                        {
                            Console.Clear();

                            Console.Write("Enter image url: ");

                            try
                            {
                                var userInput_ = Console.ReadLine();

                                using (WebClient webClient = new WebClient())
                                {
                                    byte[] data = webClient.DownloadData($"{userInput_}");

                                    using (MemoryStream memoryStream = new MemoryStream(data))
                                    {
                                        using (var image = Image.FromStream(memoryStream))
                                        {
                                            image.Save("downloaded_image.jpg", ImageFormat.Jpeg);
                                        }
                                    }
                                }

                                MemoryStream image_data = new MemoryStream(File.ReadAllBytes("downloaded_image.jpg"));
                                var result = prediction_client.ClassifyImage(project_id, model_name, image_data);

                                foreach (var item in result.Predictions)
                                {
                                    Console.WriteLine($"{item.TagName} {item.Probability:P1}");
                                }
                                Console.WriteLine("\npress any key to return to main menu");
                                Console.ReadKey();
                                MainMenu();
                            }
                            catch
                            {
                                Console.WriteLine("Error, press any key to return to main menu");
                                Console.ReadKey();
                                MainMenu();
                            }  
                        }
                        else
                        {
                            Console.WriteLine("Option not available, press any key to retry");
                            Console.ReadKey();
                            MainMenu();
                        }    
                    }
                    catch
                    {
                        Console.WriteLine("Input must be a number of the options above");
                        Console.ReadKey();
                        MainMenu();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error, {e} \nPress any key to exit application");
                    Console.ReadKey();
                }  
            }
        }
    }
}
