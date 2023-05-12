using AwesomeO.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AwesomeO
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string query = Request.QueryString["query"];
            Label1.Text = query;
            
            string connectionString = @"Data Source=DESKTOP-BJGUGNB\SQLExpress01;Initial Catalog=AwesomoTest;Integrated Security=True";
            List<DatabaseRow> rows = new List<DatabaseRow>();
            // Open SQL connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Run query with select string
                string selectQuery = "SELECT * From Captions WHERE Text LIKE @Query";
                SqlCommand command = new SqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@Query", "%" + query + "%");
                
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // We found some results, display it
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            rows.Add(new DatabaseRow(reader));
                        }
                    }
                }

                string debugText = rows.Count.ToString() + " entries found.<br>";
                foreach (DatabaseRow row in rows) 
                {
                    debugText += row.ToString() + "<br>";
                }
                Label2.Text = debugText;
            }
        }
    }
}