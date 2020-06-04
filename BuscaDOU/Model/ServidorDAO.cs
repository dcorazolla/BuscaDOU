using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace BuscaDOU.Model
{
    public class ServidorDAO : BaseDAO
    {

        

        /// <summary>
        /// Metodo construtor da classe ServidorDAO
        /// </summary>
        public ServidorDAO() : base()
        {
            CriaTabela();
        }

        /// <summary>
        /// Cria a tabela servidor
        /// </summary>
        public void CriaTabela()
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS tbl_servidor (" +
                        "int_idaservidor INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "vhr_nome VARCHAR(150), " +
                        "vhr_cpf VARCHAR(11), " +
                        "int_siape INTEGER, " +
                        "int_sigepe INTEGER, " +
                        "int_transparencia INTEGER, " +
                        "dte_atualizacaodou TIMESTAMP, " +
                        "dte_atualizacaotransparencia TIMESTAMP, " +
                        "dte_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_servidor_nome_asc ON tbl_servidor (vhr_nome ASC)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_servidor_cpf_asc ON tbl_servidor (vhr_cpf  ASC)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recupera registro de servidor pelo nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public Servidor Get(String nome)
        {
            Servidor retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            { 
                comm.CommandText = "SELECT * FROM tbl_servidor WHERE vhr_nome = '" + nome + "'";
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new Servidor(dataTable.Rows[0]);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Recupera registro de servidor pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Servidor Get(int id)
        {
            Servidor retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT * FROM tbl_servidor WHERE int_idaservidor = '" + id + "'";
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new Servidor(dataTable.Rows[0]);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Insere ou atualiza registro de servidor
        /// </summary>
        /// <param name="servidor"></param>
        public Servidor Save(Servidor servidor)
        {
            if (servidor.Id == 0)
            {
                return Add(servidor);
            }
            else
            {
                return Update(servidor);
            }
        }

        /// <summary>
        /// Insere registro de servidor
        /// </summary>
        /// <param name="servidor"></param>
        public Servidor Add(Servidor servidor)
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO tbl_servidor (vhr_nome, " +
                        "vhr_cpf, " +
                        "int_siape, " +
                        "int_sigepe, " +
                        "int_transparencia) " +
                        "VALUES (@nome, " +
                        "@cpf, " +
                        "@siape, " +
                        "@sigepe, " +
                        "@transparencia)";
                    cmd.Parameters.AddWithValue("@nome", servidor.Nome);
                    cmd.Parameters.AddWithValue("@cpf", servidor.CPF);
                    cmd.Parameters.AddWithValue("@siape", servidor.MatriculaSIAPE);
                    cmd.Parameters.AddWithValue("@sigepe", servidor.MatriculaSIGEPE);
                    cmd.Parameters.AddWithValue("@transparencia", servidor.CodigoTransparencia);
                    cmd.ExecuteNonQuery();
                }

                return Get(servidor.Nome);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Atualiza registro de servidor
        /// </summary>
        /// <param name="servidor"></param>
        public Servidor Update(Servidor servidor)
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    if (servidor.Id != 0)
                    {
                        cmd.CommandText = "UPDATE tbl_servidor SET " +
                            "vhr_nome = @nome, " +
                            "vhr_cpf = @cpf, " +
                            "int_siape = @siape, " +
                            "int_sigepe = @sigepe, " +
                            "int_transparencia = @transparencia, " +
                            "dte_atualizacaodou = @atualizacaodou, " +
                            "dte_atualizacaotransparencia = @atualizacaotransparencia " +
                            "WHERE int_idaservidor = @id";
                        cmd.Parameters.AddWithValue("@id", servidor.Id);
                        cmd.Parameters.AddWithValue("@nome", servidor.Nome);
                        cmd.Parameters.AddWithValue("@cpf", servidor.CPF);
                        cmd.Parameters.AddWithValue("@siape", servidor.MatriculaSIAPE);
                        cmd.Parameters.AddWithValue("@sigepe", servidor.MatriculaSIGEPE);
                        cmd.Parameters.AddWithValue("@transparencia", servidor.CodigoTransparencia);
                        cmd.Parameters.AddWithValue("@atualizacaodou", servidor.AtualizacaoDOU);
                        cmd.Parameters.AddWithValue("@atualizacaotransparencia", servidor.AtualizacaoTransparencia);
                        cmd.ExecuteNonQuery();
                    }
                    return Get(servidor.Id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Servidor> GetAll()
        {
            List<Servidor> retorno = new List<Servidor>();

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT * FROM tbl_servidor ORDER BY vhr_nome";
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        retorno.Add(new Servidor(row));
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Apaga registro de servidor
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            try
            {
                using (var cmd = new SQLiteCommand(Connect()))
                {
                    cmd.CommandText = "DELETE FROM tbl_servidor WHERE int_idaservidor = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
