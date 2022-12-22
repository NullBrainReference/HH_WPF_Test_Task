using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HH_WPF_Test_Task
{
    public class MyUrl
    {
        private string url;
        private int aTagsCount;

        public string Url { get { return url; } }
        public int ATagsCount { get { return aTagsCount; } }
        public bool IsUnCounted { get { return aTagsCount == -1 ? true : false; } }

        public MyUrl(string url)
        {
            this.url = url;
            this.aTagsCount = -1;
        }

        public async Task count_a_Tags()
        {
            int count = 0;
            string html = "";

            using (HttpClient client = new HttpClient())
            {
                html = await client.GetStringAsync(url);
            }

            char prev0 = ' ';
            char prev1 = ' ';
            foreach (char ch in html)
            {
                if (prev1 == '<' && prev0 == 'a')
                {
                    if (ch == ' ' || ch == '>' || ch == '/')
                        count++;
                }
                prev1 = prev0;
                prev0 = ch;
            }

            aTagsCount = count;
        }
    }
}
