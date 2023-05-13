using System;
using System.IO;
using System.Web.UI.WebControls;

namespace AwesomeO
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private const int NUM_COLUMN = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            string seasonStr = Request.QueryString["season"];
            string episodeStr = Request.QueryString["episode"];
            string startFrameStr = Request.QueryString["startFrame"];
            string endFrameStr = Request.QueryString["endFrame"];

            bool validUrl = int.TryParse(seasonStr, out int season);
            validUrl &= int.TryParse(episodeStr, out int episode);
            validUrl &= int.TryParse(startFrameStr, out int startFrame);
            validUrl &= int.TryParse(endFrameStr, out int endFrame);
            
            if(!validUrl) 
            {
                // TODO: Display text that url is not valid
                return;
            }

            // Build jpg path
            string[] framePaths = { "C:\\FHL\\SubtitlesParser\\Website\\AwesomeO\\Frames", "S" + season, "E" + episode };
            //string[] framePaths = { "Frames", "S" + season, "E" + episode };

            string framePath = Path.Combine(framePaths);
            int bufferedStart = Math.Max(0, startFrame - 10); // Cap the bufferedStart
            int bufferedEnd = endFrame + 10; // Using file exist to cap the bufferedEnd


            string testLabel = "";
            int runningColIndex = NUM_COLUMN; // Set it as NUM_COLUMN to get the first row to generate
            for(int i = bufferedStart; i < bufferedEnd; i++)
            {
                string frameNumAsString = i.ToString("D" + 5); // Convert to 5 digits
                string jpgString = "frame-" + frameNumAsString + ".jpg";
                string fullPath = Path.Combine(framePath, jpgString);

                if (File.Exists(fullPath)) 
                {
                    Image newImage = new Image();
                    newImage.ImageUrl = fullPath;
                    TableCell newCell = new TableCell();
                    newCell.Controls.Add(newImage);
                    if(runningColIndex >= NUM_COLUMN)
                    {
                        TableRow newRow = new TableRow();
                        Table1.Rows.Add(newRow);
                        runningColIndex = 0;
                    }

                    Table1.Rows[Table1.Rows.Count - 1].Cells.Add(newCell);
                    runningColIndex++;
                }
                testLabel += fullPath + "<br>";
            }

            Label1.Text = testLabel;
            
        }
    }
}