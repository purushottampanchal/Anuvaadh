using anuvadha.Models;
using Google.Cloud.Translation.V2;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Google.Cloud.TextToSpeech.V1;
using System;
using System.IO;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using NAudio.Wave;
using Whisper.net.Ggml;
using Whisper.net;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;

namespace anuvadha.Controllers
{
    //[CustomExceptionFilter]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        /* public async Task tToS(string Text)
         {
             var client = new HttpClient();
             var request = new HttpRequestMessage()
             {
                 Method = HttpMethod.Post,
                 RequestUri = new Uri("https://tts-api.ai4bharat.org/"),
                 Content = new StringContent(
             "{" +
             "\"input\": [" +
             "{" +
             "\"source\": \"" +
             Text +
             "\"" +
             "}" + "" +
             "]," +
             "\"config\": {" +
             "\"gender\": \"male\"," +
             "\"language\": " +
             "{" +
             "\"sourceLanguage\": \"hi\"" +
             "}" +
             "}"
             + "}"
             )
                 {
                     Headers = {
  ContentType = new MediaTypeHeaderValue("application/json")
  }
                 }



             };


             using (var response = await client.SendAsync(request))
             {
                 // response.EnsureSuccessStatusCode();
                 var body = await response.Content.ReadAsStringAsync();
                 //Console.WriteLine(body + "\n\n\n\n");

                 try
                 {
                     Root res = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(body);
                     Console.WriteLine(res.audio[0].audioContent);
                     var base64String = res.audio[0].audioContent;
                     byte[] binaryData = Convert.FromBase64String(base64String);
                     string filename = "C:\\Users\\raksha_ganyarpawar\\Desktop\\output.wav";
                     System.IO.File.WriteAllBytes(filename, binaryData);

                 }
                 finally
                 { }
             }



         }

         public IActionResult Index()
         {
             return View();
         }

         [HttpPost]
         public async Task<IActionResult> Index(String text, String lang)
         {


             try
             {
                 var client = TranslationClient.CreateFromApiKey("AIzaSyBh7oFZYJbqLw47GdiGUCK_miV-18DvfCI");

                 var response = client.TranslateText(text, lang);

                 ViewBag.Text = text;
                 ViewBag.translatedText = response.TranslatedText;
                 string translatedText = response.TranslatedText.Replace("\"", string.Empty).Trim();

                 Console.WriteLine(response.TranslatedText);

                 if (response.TranslatedText != null)
                 {
                     await tToS(translatedText);
                 }

                 ViewBag.OutputAudio = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\raksha_ganyarpawar\\Desktop\\output.wav"));
                 ViewBag.Data = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\raksha_ganyarpawar\\Desktop\\input.wav"));
                 //ViewBag.OutputAudio = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\purushottam_panchal\\Desktop\\output.wav"));


                 return View("temp");

             }
             catch (Exception ex)
             {
                 return RedirectToAction("Index");
             }

         }


         [HttpPost]
         public async Task<IActionResult> InputFileAsync(IFormFile postedFile)
         {
             string fileName = Path.GetFileName(postedFile.FileName);
             using (FileStream stream = new FileStream("C:\\Users\\raksha_ganyarpawar\\Desktop\\input.wav", FileMode.Create))
             {
                 postedFile.CopyTo(stream);
             }


             ViewBag.Data = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\raksha_ganyarpawar\\Desktop\\input.wav"));
             return View("Dummy");
         }



         [HttpPost]
         public async Task<IActionResult> PlayerAsync(IFormFile postedFile, string languages)
         {

             string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads");
             if (!Directory.Exists(path))
             {
                 Directory.CreateDirectory(path);
             }

             string fileName = Path.GetFileName(postedFile.FileName);
             using (FileStream stream = new FileStream("C:\\Users\\raksha_ganyarpawar\\Desktop\\input.wav", FileMode.Create))
             {
                 postedFile.CopyTo(stream);
             }
             string AudioPath = Path.Combine(path, fileName);



             var client = new HttpClient();



             var filePath = "C:\\Users\\raksha_ganyarpawar\\Desktop\\input.wav";


             //s2t
             using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://27b8-35-247-132-232.ngrok.io/upload"))
             {
                 var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                 fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");



                 var formData = new MultipartFormDataContent();
                 formData.Add(fileContent, "audio", "audioshashitharoor.wav");



                 request.Content = formData;
                 var response = await client.SendAsync(request);
                 var responseContent = await response.Content.ReadAsStringAsync();

                 Console.WriteLine(responseContent);
                 var inputText = JsonSerializer.Deserialize<InputText>(responseContent);
                 Console.WriteLine(inputText.message);
                 Console.WriteLine(responseContent);

                 //ViewBag.Data = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\purushottam_panchal\\Desktop\\input.wav"));
                 //ViewBag.OutputAudio = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\purushottam_panchal\\Desktop\\output.wav"));

                 //var modelName = "C:\\Users\\purushottam_panchal\\Desktop\\ggml-base.bin";
                 //if (!System.IO.File.Exists(modelName))
                 //{
                 //    using var modelStream = await Whisper.net.Ggml.WhisperGgmlDownloader.GetGgmlModelAsync(GgmlType.Medium);
                 //    using var fileWriter = System.IO.File.OpenWrite(modelName);
                 //    await modelStream.CopyToAsync(fileWriter);
                 //}

                 //var whisperFactory = WhisperFactory.FromPath(modelName);



                 //var processor = whisperFactory.CreateBuilder()
                 //.WithLanguage("en")
                 //.WithTranslate()
                 //.Build();


                 ////string wavFile = processAudio(AudioPath);
                 ////FileStream InputFile = System.IO.File.OpenRead(wavFile);

                 //FileStream InputFile = System.IO.File.OpenRead(AudioPath);

                 //var segments = processor.ProcessAsync(InputFile, CancellationToken.None);

                 //string text = "";

                 //await foreach (var segment in segments)
                 //{
                 //    Console.WriteLine($"{segment.Text}");
                 //    text += segment.Text;

                 //}
                 // Console.WriteLine(segments);
                 string text = inputText.message;
                 await Index(text, languages);
                 try
                 {
                     await clone(robotic: @"C:\Users\raksha_ganyarpawar\Desktop\output.wav",
                      original: @"C:\Users\raksha_ganyarpawar\Desktop\input.wav");
                 }catch(Exception e)
                 {
                     return View("Index");
                 }


                 ViewBag.OutputAudio = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\raksha_ganyarpawar\\Desktop\\output.wav"));
                 ViewBag.Data = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\raksha_ganyarpawar\\Desktop\\input.wav"));
                 ViewBag.ClonedAudio = "data:audio/wav;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\Users\\raksha_ganyarpawar\\Desktop\\cloned.wav"));


                 return View("temp");

                // return View("speechTospeech");
             }


         }

         static async Task clone(string robotic, string original)
         {

             //cloning
             var client = new HttpClient();
             using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://6d26-35-202-169-87.ngrok.io/upload"))
             {
                 var RoboticFileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(robotic));
                 var OriginaltFileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(original));

                 RoboticFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");
                 OriginaltFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

                 var formData = new MultipartFormDataContent();
                 formData.Add(RoboticFileContent, "audio", "robotic.wav");
                 formData.Add(OriginaltFileContent, "audio", "original.wav");

                 request.Content = formData;
                 var response = await client.SendAsync(request);

                 var responseContent = await response.Content.ReadAsStreamAsync();
                 Console.WriteLine(responseContent);

                 var outFileName = "C:\\Users\\raksha_ganyarpawar\\Desktop\\cloned.wav";

                 try
                 {
                     WaveFileReader waveFileReader = new WaveFileReader(responseContent);
                     WaveFileWriter.CreateWaveFile(outFileName, waveFileReader);
                 }catch(Exception e)
                 {

                 }


             }
         }

         public IActionResult Dummy()
         {
             return View();
         }

         public IActionResult speechTospeech()
         {
             return View();
         }
 */


        private readonly string GOOGLE_TRANSLATION_API_KEY = "AIzaSyBh7oFZYJbqLw47GdiGUCK_miV-18DvfCI";
        private static string S2T_API_URL = "http://127.0.0.1:3000/upload";
        private static string CLONING_API_URL = "http://127.0.0.1:5000/upload";


        public async Task<byte[]> tToS(string text)
        {
            var client = new HttpClient();
            SpeechToTextRequestModel reqContent = new SpeechToTextRequestModel()
            {
                input = new()
                {
                    new ReqInput() { source = text }
                },
                config = new()
                {
                    gender = "male", //female
                    language = new()
                    {
                        sourceLanguage = "hi"
                    }
                }

            };
            string stringCOntent = JsonSerializer.Serialize(reqContent);
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://tts-api.ai4bharat.org/"),
                Content = new StringContent(stringCOntent)
                {
                    Headers = {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };

            using (var response = await client.SendAsync(request))
            {

                response.EnsureSuccessStatusCode();
                var resString = await response.Content.ReadAsStringAsync();

                SpeechToTextResponceModel responceModel = Newtonsoft.Json.JsonConvert.
                    DeserializeObject<SpeechToTextResponceModel>(resString);

                var base64String = responceModel.audio[0].audioContent;
                byte[] binaryData = Convert.FromBase64String(base64String);

                //string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads");
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

                //string filename = path + "robotic_translated_voice.wav";
                //System.IO.File.WriteAllBytes(filename, binaryData);

                return binaryData;

            }



        }

        public async Task<IActionResult> IndexAsync()
        {
            return View();
        }

        

        private async Task<string> TransScript(string fileName, string AudioPath)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), S2T_API_URL))
                {
                    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(AudioPath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");
                    var formData = new MultipartFormDataContent();

                    formData.Add(fileContent, "audio", "" + fileName);
                    request.Content = formData;

                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();

                        var responseContent = await response.Content.ReadAsStringAsync();
                        var TranscriptedText = JsonSerializer.Deserialize<InputText>(responseContent);
                        string transcript = TranscriptedText.message;

                        return transcript;
                    };

                };
            };
        }

        private string[] TranslateText(string text, string targetLanguage)
        {
            string[] result = new string[2];
            using (var client = TranslationClient.CreateFromApiKey(GOOGLE_TRANSLATION_API_KEY))
            {
                var response = client.TranslateText(text, targetLanguage);
                result[0] = response.TranslatedText;
                result[1] = response.OriginalText;
                return result;
            };


        }


        [HttpPost]
        public async Task<IActionResult> PlayerAsync(IFormFile PostedAudioFile,string languages)
        {


            string filenamePrefix = PostedAudioFile.FileName.Replace(".wav", "");

            string RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\AudioFiles\\{filenamePrefix}\\");
            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);
            }


            string OriginalfileName = filenamePrefix + "_Original.wav";


            //saving original
            string OriginalAudioFilePath = Path.Combine(RootDirectory, OriginalfileName);
            using (FileStream stream = new FileStream(OriginalAudioFilePath, FileMode.Create))
            {
                PostedAudioFile.CopyTo(stream);
            }
            _logger.LogInformation("FileUploaded : " + OriginalAudioFilePath);

            _logger.LogInformation(">>>>>Transcripting");
            //>>>transcript
            string transcript = await TransScript(OriginalfileName, OriginalAudioFilePath);
            _logger.LogInformation("TransScription : " + transcript);

            _logger.LogInformation(">>>>>Translating to !"+languages);
            //>>>Translate
            //translations[0] -->translatied text
            //translations[1] -->Original Text
            string TargetLanguage = languages;
            var translations = TranslateText(transcript, TargetLanguage);
            string TranslatedText = translations[0];
            _logger.LogInformation("Translation done !");

            _logger.LogInformation(">>>>>Generating speech from transcript!");
            //>>>textToSpeech --robotic
            var RoboticAudioFileBytes = await tToS(TranslatedText);
            string RoboticFileName = filenamePrefix + "_Translated_robotic.wav";
            string RoboticFilePath = Path.Combine(RootDirectory, RoboticFileName);
            System.IO.File.WriteAllBytes(RoboticFilePath, RoboticAudioFileBytes);
            _logger.LogInformation("Robotic Voice generated at " + RoboticFilePath);

            _logger.LogInformation(">>>>>Cloning voice!");
            //>>>CloneVoice
            Stream clonedAudioStream = await clone(RoboticFilePath, OriginalAudioFilePath);
            string clonedFileName = filenamePrefix + "_Translated_cloned.wav";
            string clonedAudioFilePath = Path.Combine(RootDirectory, clonedFileName);

            WaveFileReader waveFileReader = new WaveFileReader(clonedAudioStream);
            WaveFileWriter.CreateWaveFile(clonedAudioFilePath, waveFileReader);
            _logger.LogInformation("cloned voice audio file saved at " + clonedAudioFilePath);



            ViewBag.RoboticTranslated = "data:audio/wav;base64," +
                Convert.ToBase64String(
                    System.IO.File.ReadAllBytes(RoboticFilePath));

            ViewBag.OriginalFile = "data:audio/wav;base64," +
                Convert.ToBase64String(System.IO.File.ReadAllBytes(OriginalAudioFilePath));

            ViewBag.ClonedAudio = "data:audio/wav;base64," +
                Convert.ToBase64String(System.IO.File.ReadAllBytes(clonedAudioFilePath));

            ViewBag.TranslatedText = TranslatedText;
            ViewBag.Transcript = transcript;

            return View("temp");

        }


       

        static async Task<Stream> clone(string robotic, string original)
        {

            //cloning
            var client = new HttpClient();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"),
                "http://127.0.0.1:5000/upload"))
            {
                var RoboticFileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(robotic));
                var OriginaltFileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(original));

                RoboticFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");
                OriginaltFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

                var formData = new MultipartFormDataContent();
                formData.Add(RoboticFileContent, "audio", "robotic.wav");
                formData.Add(OriginaltFileContent, "audio", "original.wav");

                request.Content = formData;
                var response = await client.SendAsync(request);

                var responseContent = await response.Content.ReadAsStreamAsync();
                return responseContent; ;
              

            }
        }

        public IActionResult Dummy()
        {
            return View();
        }

        public IActionResult Dummy2()
        {
            return View();
        }


    }
}