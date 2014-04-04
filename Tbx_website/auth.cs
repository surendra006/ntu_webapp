using System;
using System.Data.SqlClient;
using System.Configuration;

public class auth
{
    static readonly string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
    SqlCommand myCommand;

    SqlConnection conn = new SqlConnection(connectionString);
    public bool Authenticate(string token, ref string gid, ref string uid)
    {
        uid = "";
        gid = "";
        if ((string.IsNullOrEmpty(token)))
        {
            return false;
        }

        SqlDataReader reader = null;
        string secureToken = "";
        bool reply = false;


        myCommand = new SqlCommand("SELECT * FROM account WHERE securityToken='" + token + "'", conn);
        try
        {
            conn.Open();
            reader = myCommand.ExecuteReader();
            if ((reader.HasRows))
            {
                reply = true;
                while (reader.Read())
                {
                    uid = reader["userid"] + "";
                    gid = reader["groupid"] + "";
                }
            }
        }
        catch (Exception err)
        {
            // Handle an error by displaying the information.
        }
        conn.Close();

        return reply;
    }
}
