using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Commons.CustomDatabaseManager
{
    public class CustomDatabase
    {
        static string getCS ()
        {
            //return string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};Application Name=SsnService;MultipleActiveResultSets=True",
            //    Config.sqlServerName, Config.sqlDatabaseName, Config.sqlUser, Config.sqlPass);

            return "";
        }

        public static bool ejecutar (string cs, string sql, ref DataSet resultado, ref string error)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cs))
                {
                    conexion.Open();

                    SqlCommand     cmd     = new SqlCommand(sql, conexion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    resultado = new DataSet();

                    adapter.Fill(resultado);

                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                error = "DBHelper.ejecutar -> " + ex.Message;
            }

            return (0 == error.Length);
        }

        public static bool ejecutar (string cs, string sql, ref string error)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cs))
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand(sql, conexion);

                    cmd.ExecuteNonQuery();

                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                error = "DBHelper.ejecutar -> " + ex.Message;
            }

            return (0 == error.Length);
        }

        public static bool ejecutar (string cs, string sp, List<SqlParameter> parametros, ref DataSet resultado, ref string error)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cs))
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand(sp, conexion);

                    cmd.CommandTimeout = 60;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parametros.ToArray());

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(resultado);

                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                error = "DBHelper.ejecutar -> " + ex.Message;
            }

            return (0 == error.Length);
        }

        public static bool ejecutar (string cs, string sp, List<SqlParameter> parametros, ref string error)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cs))
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand(sp, conexion);

                    cmd.CommandTimeout = 60;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parametros.ToArray());

                    cmd.ExecuteNonQuery();

                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                error = "DBHelper.ejecutar -> " + ex.Message;
            }

            return (0 == error.Length);

        }

        public static void agregarParametro (ref List<SqlParameter> lista, string nombre, SqlDbType tipo, object valor)
        {
            SqlParameter parametro = new SqlParameter(nombre, tipo);

            parametro.Value = valor;

            lista.Add(parametro);
        }
    }
}
