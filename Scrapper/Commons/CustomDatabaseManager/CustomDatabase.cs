using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Commons.CustomDatabaseManager
{
    public class CustomDatabase
    {
        SqlConnection   connection       = null;
        string          connectionString = string.Empty;

        public CustomDatabase (string server, string dbname, string usr, string pwd)
        {
            connectionString = string.Format(
                "Data Source={0};Initial Catalog={1};User id={2};Password={3};Application Name=WebScrapper",
                server, dbname, usr, pwd
            );

            connection = new SqlConnection(connectionString);
        }


        public bool open (ref string error)
        {
            if (connection == null)
                connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                error = "CustomDatabase.Open -> " + ex.Message;
            }

            return (0 == error.Length);
        }

        public void close ()
        {
            try
            {
                if (connection != null)
                    connection.Close();

                connection = null;
            }
            catch 
            {
                //
            }
        }

        public bool execute (string sql, ref DataSet dsResponse, ref string error)
        {
            if (0 == error.Length)
                canExecut(ref error);

            if (0 == error.Length)
            {
                try
                {
                    SqlCommand      cmd     = new SqlCommand(sql, connection);
                    SqlDataAdapter  adapter = new SqlDataAdapter(cmd);

                    dsResponse = new DataSet();

                    adapter.Fill (dsResponse);
                }
                catch (Exception ex)
                {
                    error = "CustomDatabase.execute -> " + ex.Message;
                }
            }

            return (0 == error.Length);
        }

        public bool execute (string sql, ref string error)
        {
            if (0 == error.Length)
                canExecut(ref error);

            if (0 == error.Length)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    error = "CustomDatabase.execute -> " + ex.Message;
                }
            }

            return (0 == error.Length);
        }

        public bool execute (string sp, List<SqlParameter> parameters, ref DataSet dsResponse, ref string error)
        {
            if (0 == error.Length)
                canExecut(ref error);

            if (0 == error.Length)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sp, connection);

                    cmd.CommandTimeout = 60;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill (dsResponse);
                }
                catch (Exception ex)
                {
                    error = "CustomDatabase.execute -> " + ex.Message;
                }
            }

            return (0 == error.Length);
        }

        public bool execute (string sp, List<SqlParameter> parameters, ref string error)
        {
            if (0 == error.Length)
                canExecut(ref error);

            if (0 == error.Length)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sp, connection);

                    cmd.CommandTimeout = 60;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters.ToArray());

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    error = "CustomDatabase.execute -> " + ex.Message;
                }
            }

            return (0 == error.Length);
        }

        public void addParameter (ref List<SqlParameter> parameters, string name, SqlDbType type, object value)
        {
            SqlParameter parameter = new SqlParameter(name, type);
            parameter.Value = value;

            if (parameters == null)
                parameters = new List<SqlParameter>();

            parameters.Add (parameter);
        }


        bool canExecut (ref string error)
        {
            if (0 == error.Length && connection == null)
                error = "There isn't connection";

            if (0 == error.Length && connection.State != ConnectionState.Open)
                error = "Connection isn't open";

            return (0 == error.Length);
        }
    }
}
