using CarlosAg.ExcelXmlWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Octokit.Example
{
    class IssuesH
    {
        public List<string> Open { get; set; }
        public List<string> Closed { get; set; }
        public string Error { get; set; }

        public IssuesH()
        {
            Open = new List<string>();
            Closed = new List<string>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //октомври - 1
            //ноември - 2
            //декември - 3

            //януари - 4?? squash test

            var task = GetIssuesForMilestone("3");
            task.Wait();

            IssuesH result = task.Result;

            if (!string.IsNullOrEmpty(result.Error))
            {
                Console.WriteLine(result.Error);
            }
            else
            {
                ExportToExcel(result);
            }

            Console.Read();
        }

        static void ExportToExcel(IssuesH data)
        {
            try
            {
                Workbook book = new Workbook();
                Worksheet sheet = book.Worksheets.Add("sheet 1");
                WorksheetStyle style = book.Styles.Add("HeaderStyle");
                style.Font.Bold = true;
                style.Alignment.WrapText = true;
                style = book.Styles.Add("Default");
                style.Font.FontName = "Tahoma";
                style.Alignment.WrapText = true;
                style.Alignment.Horizontal = StyleHorizontalAlignment.Justify;
                style.Alignment.Vertical = StyleVerticalAlignment.Justify;
                style.Font.Size = 10;

                WorksheetRow row = sheet.Table.Rows.Add();
                row.Cells.Add(new WorksheetCell("Отворени:", "HeaderStyle"));

                foreach (var item in data.Open)
                {
                    row = sheet.Table.Rows.Add();
                    row.Cells.Add(item);
                }

                row = sheet.Table.Rows.Add();
                row = sheet.Table.Rows.Add();
                row.Cells.Add(new WorksheetCell("Затворени:", "HeaderStyle"));

                foreach (var item in data.Closed)
                {
                    row = sheet.Table.Rows.Add();
                    row.Cells.Add(item);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    book.Save("result.xml");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<IssuesH> GetIssuesForMilestone(string milestone)
        {
            IssuesH result = new IssuesH();
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
                    //Console.WriteLine("No open issues");
                }
                else
                {
                    //Console.WriteLine("Open issues ({0}):", openIssues.Count);
                    foreach (var item in openIssues)
                    {
                        result.Open.Add(item.Title);
                        //Console.WriteLine(item.Title);
                    }
                }

                //Console.WriteLine("-----------------------");

                if (closedIssues.Count == 0)
                {
                    //Console.WriteLine("No open issues");
                }
                else
                {
                    //Console.WriteLine("Closed issues ({0}):", closedIssues.Count);
                    foreach (var item in closedIssues)
                    {
                        result.Closed.Add(item.Title);
                        //Console.WriteLine(item.Title);
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                result.Error = ex.Message;
            }
            finally
            {
                //Console.WriteLine();
                //Console.WriteLine("Press any key to quit...");
            }

            return result;
        }
    }
}
