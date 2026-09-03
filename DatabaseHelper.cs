using System;
using System.Data;
using System.Data.SqlClient;

namespace CampManagementSystem
{
    public static class DatabaseHelper
    {
        // ====== Local SQL Server connection string ======
        // Default: LocalDB. Change if you use SQL Server Express or full SQL Server.
        // Examples:
        //   LocalDB:  @"Server=(localdb)\MSSQLLocalDB;Database=CampManagementDB;Integrated Security=True;"
        //   Express:  @"Server=.\SQLEXPRESS;Database=CampManagementDB;Integrated Security=True;"
        //   SQL Auth: @"Server=.\SQLEXPRESS;Database=CampManagementDB;User Id=sa;Password=yourpass;"
        private static readonly string connString =
            @"Server=(localdb)\MSSQLLocalDB;Database=CampManagementDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connString);
        }

        // Execute INSERT / UPDATE / DELETE — returns rows affected
        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Execute SELECT — returns a DataTable filled by SqlDataAdapter
        public static DataTable ExecuteQuery(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Execute scalar (e.g. SELECT COUNT(*))
        public static object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}