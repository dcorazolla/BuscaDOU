using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaDOU.Model
{
    public class BaseDAO
    {

        public const string path = ".\\dados";
        public string nomeBanco = "base";
        public SQLiteConnection sqliteConnection = null;

        public BaseDAO()
        {
        }

        /// <summary>
        /// Conecta a banco e retorna conexao
        /// </summary>
        /// <returns>SQLiteConnection</returns>
        public SQLiteConnection Connect()
        {
            try
            {
                // se nao existir banco de dados cria
                if (!System.IO.File.Exists(path + "\\" + nomeBanco + ".db")) CriaBanco();
                // se nao estiver conectado, conecta no banco
                if (sqliteConnection == null)
                {
                    sqliteConnection = new SQLiteConnection("Data Source=" + path + "\\" + nomeBanco + ".db; Version=3;");
                    sqliteConnection.Open();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao conectar arquivo " + path + "\\" + nomeBanco + ".db" + " - " + e.Message);
            }
            return sqliteConnection;
        }

        /// <summary>
        /// Cria banco de dados
        /// </summary>
        public void CriaBanco()
        {
            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                SQLiteConnection.CreateFile(@path + "\\" + nomeBanco + ".db");
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao criar arquivo " + path + "\\" + nomeBanco + ".db" + " - " + e.Message);
            }
        }

    }
}
