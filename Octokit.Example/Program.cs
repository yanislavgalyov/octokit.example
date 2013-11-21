using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Octokit.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //октомври - 1
            //ноември - 2
            //декември - 3
            GetIssuesForMilestone("2");

            Console.Read();
        }

        static async void GetIssuesForMilestone(string milestone)
        {
            try
            {
                string login, pass;
                Console.Write("login:");
                login = Console.ReadLine();

                Console.Write("pass:");
                pass = Console.ReadLine();

                GitHubClient github = new GitHubClient(new ProductHeaderValue("GetMilestoneReport"))
                {
                    Credentials = new Credentials(login, pass)
                };

                var oir = new RepositoryIssueRequest();
                oir.Filter = IssueFilter.All;
                oir.State = ItemState.Open;
                oir.Milestone = milestone;

                var cir = new RepositoryIssueRequest();
                cir.Filter = IssueFilter.All;
                cir.State = ItemState.Closed;
                cir.Milestone = milestone;

                IReadOnlyList<Issue> openIssues = await github.Issue.GetForRepository("angelyordanov", "ems", oir);
                IReadOnlyList<Issue> closedIssues = await github.Issue.GetForRepository("angelyordanov", "ems", cir);

                if (openIssues.Count == 0)
                {
                    Console.WriteLine("No open issues");
                }
                else
                {
                    Console.WriteLine("Open issues ({0}):", openIssues.Count);
                    foreach (var item in openIssues)
                    {
                        Console.WriteLine(item.Title);
                    }
                }

                Console.WriteLine("-----------------------");

                if (closedIssues.Count == 0)
                {
                    Console.WriteLine("No open issues");
                }
                else
                {
                    Console.WriteLine("Closed issues ({0}):", closedIssues.Count);
                    foreach (var item in closedIssues)
                    {
                        Console.WriteLine(item.Title);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to quit...");
            }
        }
    }
}
