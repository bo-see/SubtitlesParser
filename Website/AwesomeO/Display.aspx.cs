using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace AwesomeO
{
    public partial class Display : System.Web.UI.Page
    {
        private const int NUM_COLUMN = 3;

        protected void Page_Load(object sender, EventArgs e)
        {
            string seasonStr = Request.QueryString["season"];
            string episodeStr = Request.QueryString["episode"];
            string startFrameStr = Request.QueryString["startFrame"];
            string endFrameStr = Request.QueryString["endFrame"];
            string textToDisplay = Request.QueryString["text"];

            bool validUrl = int.TryParse(seasonStr, out int season);
            validUrl &= int.TryParse(episodeStr, out int episode);
            validUrl &= int.TryParse(startFrameStr, out int startFrame);
            validUrl &= int.TryParse(endFrameStr, out int endFrame);
            
            if(!validUrl) 
            {
                // TODO: Display text that url is not valid
                return;
            }

            Label1.Text = textToDisplay;

            // Build jpg path
            //string[] framePaths = { "C:\\FHL\\SubtitlesParser\\Website\\AwesomeO\\Frames", "S" + season, "E" + episode };
            string[] framePaths = { "http://localhost:8080/", "S" + season, "E" + episode }; // Need to run local server from whereever the folder is

            Uri framePath = new Uri(Path.Combine(framePaths));
            int bufferedStart = Math.Max(0, startFrame - 10); // Cap the bufferedStart
            int bufferedEnd = endFrame + 10; // Using file exist to cap the bufferedEnd

            int runningColIndex = NUM_COLUMN; // Set it as NUM_COLUMN to get the first row to generate
            for(int i = bufferedStart; i < bufferedEnd; i++)
            {
                string frameNumAsString = i.ToString("D" + 5); // Convert to 5 digits
                string jpgString = "frame-" + frameNumAsString + ".jpg";
                string fullPath = framePath + "/" + jpgString;

                // Probably should wrap this in a safe bounds checkh
                Image newImage = new Image();
                newImage.ImageUrl = fullPath;
                newImage.AlternateText = frameNumAsString;
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
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string startFrameText = TextBox1.Text;
            string endFrameText = TextBox2.Text;

            bool validSelection = int.TryParse(startFrameText, out int startFrame);
            validSelection &= int.TryParse(endFrameText, out int endFrame);

            int numFrame = endFrame - startFrame;
            if (!validSelection || numFrame < 0)
            {
                // TODO: output error
                return;
            }

            Response.Redirect("GifCreationTest.aspx?startFrame=" + startFrame + "&numFrame=" + numFrame + "&overlayText=" + HttpUtility.UrlEncode(Label1.Text));
        }
    }
}