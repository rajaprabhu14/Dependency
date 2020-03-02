using Dependency_Solution.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web.Http;

namespace Dependency_Solution.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get()
        {
            List<string> myCollection = new List<string>();

            //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Input.json");
            //string[] files = File.ReadAllLines(path);

            StreamReader r = new StreamReader(@"C:\Users\rajap\source\repos\Dependency Solution\Input.json");

            var json = r.ReadToEnd();
            List<DependencyInput> f = JsonConvert.DeserializeObject<List<DependencyInput>>(json);
            string[] inputs = f.SelectMany(x => x.Inputs, (x, y) => y.Input).ToArray();
            var dict = inputs.ToDictionary(s => s[0], v => v.Substring(2).Where(Char.IsLetter).ToArray());
            

            IEnumerable<char> Dependencies(char input)
            {
                yield return input;
                if (dict.TryGetValue(input, out char[] childDepends))
                {
                    foreach (char result in childDepends.SelectMany(Dependencies))
                    {
                        yield return result;
                    }
                }
            }

            foreach (char s in inputs.Select(x => x[0]))
            {
                myCollection.Add(s + " " + String.Join(" ", dict[s].SelectMany(Dependencies).Distinct().OrderBy(x => x)));
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
