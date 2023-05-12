using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AwesomeO
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //String startframe = Request.QueryString["startframe"];
            //String numFrames = Request.QueryString["numFrames"];
            //String overlayText = Request.QueryString["overlayText"];

            //string filename = MD5.Create(Guid.NewGuid().ToString()).ToString();

            //create text file with text, same name as gif but with .txt extension. pass ffmpeg the text file for overlay (so we can handle UTF8 chars)

            //Need to get pathing working. Need absolute pathing for the process start, find out absolute paths for our website.
            var process = System.Diagnostics.Process.Start("D:\\repos\\awesomeo\\SubtitlesParser\\Website\\AwesomeO\\ffmpeg.exe", "-start_number 12069 -i D:\\repos\\awesomeo\\SubtitlesParser\\Website\\AwesomeO\\Episodes\\S25\\E01\\frame-%05d.jpg -vframes 50 -framerate 24 -y D:\\repos\\awesomeo\\SubtitlesParser\\Website\\AwesomeO\\output3.gif");

            process.WaitForExit();

            Image1.ImageUrl = ".\\output3.gif";

        }
    }
}