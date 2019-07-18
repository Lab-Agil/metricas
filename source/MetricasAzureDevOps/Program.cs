using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MetricasAzureDevOps
{
    class Program
    {
        //static async Task Main(string[] args)
        static async Task Main()
        {
            Console.WriteLine("Iniciado...");

            await RunGetBugsQueryUsingClientLib();

            Console.WriteLine("Finalizado!");
            Console.WriteLine("Pressione ENTER para sair");
            Console.ReadLine();
        }

        public static async Task RunGetBugsQueryUsingClientLib()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var projetoDesejado = configuration.GetSection("ProjetoDesejado").Value;

            var sources = configuration.GetSection("Projetos:" + projetoDesejado).GetChildren().ToList();

            var uri = new Uri(sources.FirstOrDefault(x => x.Key == "UriString").Value);
            var personalAccessToken = sources.FirstOrDefault(x => x.Key == "PersonalAccessToken").Value;
            var project = sources.FirstOrDefault(x => x.Key == "Project").Value;

            var credentials = new VssBasicCredential("", personalAccessToken);

            var wiqlQuery = configuration.GetSection("WiqlQuery").Value;

            var wiql = new Wiql()
            {
                Query = wiqlQuery
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                WorkItemQueryResult workItemQueryResult = await workItemTrackingHttpClient.QueryByWiqlAsync(wiql);

                if (workItemQueryResult.WorkItems.Count() > 0)
                {
                    List<int> list = new List<int>();
                    string itemIdAnterior = null;
                    string itemId;
                    string boardColumn;
                    string boardColumnAnterior = null;

                    bool preencher = true;

                    using (var writer = new StreamWriter("./output.csv"))
                        foreach (var item in workItemQueryResult.WorkItems)
                        {
                            var Revions = await workItemTrackingHttpClient.GetRevisionsAsync(item.Id);

                            foreach (var r in Revions)
                            {
                                preencher = true;
                                var Type = r.Fields.Where(p => p.Key == "System.WorkItemType").ToDictionary(p => p.Key, p => p.Value).FirstOrDefault();
                                var Title = r.Fields.Where(p => p.Key == "System.Title").ToDictionary(p => p.Key, p => p.Value).FirstOrDefault();
                                var CreatedDate = r.Fields.Where(p => p.Key == "System.CreatedDate").ToDictionary(p => p.Key, p => p.Value).FirstOrDefault();
                                var Interation = r.Fields.Where(p => p.Key == "System.IterationPath").ToDictionary(p => p.Key, p => p.Value).FirstOrDefault();
                                var BoardColumn = r.Fields.Where(p => p.Key == "System.BoardColumn").ToDictionary(p => p.Key, p => p.Value).FirstOrDefault();
                                var ChangedDate = r.Fields.Where(p => p.Key == "System.ChangedDate").ToDictionary(p => p.Key, p => p.Value).FirstOrDefault();

                                itemId = item.Id.ToString();
                                boardColumn = BoardColumn.Value == null ? "-1" : BoardColumn.Value?.ToString();

                                if (itemId.Equals(itemIdAnterior) && boardColumn.Equals(boardColumnAnterior)) preencher = false;

                                var linha = string.Join(";", new string[] {
                                  item.Id.ToString()
                                , Type.Value.ToString()
                                , Title.Value.ToString()
                                , CreatedDate.Value.ToString()
                                , Interation.Value?.ToString()
                                , BoardColumn.Value?.ToString()
                                , ChangedDate.Value?.ToString()
                            });

                                itemIdAnterior = itemId;
                                boardColumnAnterior = BoardColumn.Value == null ? "-1" : BoardColumn.Value?.ToString(); ;

                                //if (preencher) Console.WriteLine(linha);
                                if (preencher) writer.WriteLine(linha);
                            }
                        }
                }
            }
        }
    }
}