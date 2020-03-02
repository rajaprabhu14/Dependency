using Dependency_Solution.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Dependency_Solution.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get()
        {
            List<string> myCollection = new List<string>();

            // For demo purpose, I have hard coded the input json below.
            string lobjFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            StreamReader lobjStreamReader = new StreamReader(lobjFilePath + "Input.json");

            var lobjStreamReaderJson = lobjStreamReader.ReadToEnd();
            List<DependencyInput> lobjDeserializeJson = JsonConvert.DeserializeObject<List<DependencyInput>>(lobjStreamReaderJson);
            string[] lobjJsonInputs = lobjDeserializeJson.SelectMany(element => element.Inputs, (acc, target) => target.Input).ToArray();
            var LobjDictionary = lobjJsonInputs.ToDictionary(index => index[0], items => items.Substring(2).Where(Char.IsLetter).ToArray());

            IEnumerable<char> Dependencies(char input)
            {
                yield return input;
                if (LobjDictionary.TryGetValue(input, out char[] childDepends))
                {
                    foreach (char result in childDepends.SelectMany(Dependencies))
                    {
                        yield return result;
                    }
                }
            }

            foreach (char s in lobjJsonInputs.Select(data => data[0]))
            {
                myCollection.Add(s + " " + String.Join(" ", LobjDictionary[s].SelectMany(Dependencies).Distinct().OrderBy(data => data)));
            }

            using (var file = new StreamWriter(@"C:\Users\Public\token.txt"))
            {
                foreach (string line in myCollection)
                {
                    file.WriteLine(line);
                }
            };

            var dataBytes = File.ReadAllBytes(@"C:\Users\Public\token.txt");
            var dataStream = new MemoryStream(dataBytes);

            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = "token.txt";

            return httpResponseMessage;
        }
    }
}
