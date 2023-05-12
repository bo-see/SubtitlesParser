using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AwesomeO.Model
{
    public class DatabaseRow
    {
        public Int32 EntryNum;
        public Int16 Season;
        public Int16 EpisodeNumber;
        public string StartTimestamp;
        public string EndTimestamp;
        public Int32 StartFrame;
        public Int32 EndFrame;
        public string Text;

        public DatabaseRow() { }
        public DatabaseRow(SqlDataReader reader)
        {
            this.EntryNum = reader.GetInt32(0);
            this.Season = reader.GetInt16(1);
            this.EpisodeNumber = reader.GetInt16(2);
            this.StartTimestamp = reader.GetString(3);
            this.EndTimestamp = reader.GetString(4);
            this.StartFrame = reader.GetInt32(5);
            this.EndFrame = reader.GetInt32(6);
            this.Text = reader.GetString(7);
        }

        override public string ToString()
        {
            return $"EntryNum: {this.EntryNum} | Season: {this.Season} | EpisodeNumber: {this.EpisodeNumber} | StartTimestamp: {this.StartTimestamp} | EndTimestamp: {this.EndTimestamp} | StartFrame: {this.StartFrame} | EndFrame: {this.EndFrame} | Text: {this.Text}";
        }
    }
}