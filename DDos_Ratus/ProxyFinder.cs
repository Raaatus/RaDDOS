using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DDos_Ratus
{
    public static class ProxyFinder
    {
        private static readonly HttpClient client = new HttpClient();


        
        public static async Task FindProxiesAsync()
        {
            try
            {
                string url = "https://www.proxyscan.io/api/proxy?last_check=500&ping=600&limit=20&type=http"; // Remplacez cette URL par l'URL de l'API de recherche de proxys
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var proxyData = JsonConvert.DeserializeObject<List<ProxyData>>(jsonString);

                    if (proxyData != null)
                    {
                        foreach (var data in proxyData)
                        {
                            if (IPAddress.TryParse(data.IP, out IPAddress ipAddress) && data.Port > 0)
                            {
                                Form1.proxyList.Add((data.IP, data.Port));
                            }
                        }

                        Console.WriteLine("[+] Proxies ajoutés : " + proxyData.Count);
                    }
                    else
                    {
                        Console.WriteLine("[-] Aucun proxy trouvé.");
                    }
                }
                else
                {
                    Console.WriteLine("[-] Erreur lors de la requête de recherche de proxys : " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Erreur lors de la recherche de proxys : " + ex.Message);
            }

        }
        public static void LoadProxyListFromFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2 && IPAddress.TryParse(parts[0], out IPAddress ipAddress) && int.TryParse(parts[1], out int port))
                    {
                        Form1.proxyList.Add((parts[0], port));
                    }
                }

                Console.WriteLine("[+] Proxies ajoutés depuis le fichier : " + lines.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Erreur lors du chargement de la liste de proxys depuis le fichier : " + ex.Message);
            }
        }

        private class ProxyData
        {
            public string IP { get; set; }
            public int Port { get; set; }
        }

       
    }
}
