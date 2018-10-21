using MySql.Data.MySqlClient;
using SqlKata.Compilers;

namespace UServer.Database
{
    public static class Adapter
    {
        public static MySqlConnection Connection { get; private set; }
        public static MySqlCompiler Compiler { get; private set; }

        /// <summary>
        /// Recommneded to be only called once to initialize the compiler/connection instances
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="db"></param>
        /// <param name="SslMode"></param>
        public static void Initialize(string host, int port, string user, string pwd, string db, string SslMode = "None")
        {
            Compiler = new MySqlCompiler();
            Connection = new MySqlConnection($"Host={host};Port={port};User={user};Password={pwd};Database={db};SslMode={SslMode};convert zero datetime=True");
        }


       
    }
}
