using System;
using System.Collections.Generic;
using System.Data;

namespace BuscaDOU.Model
{
    public class ServidorPublicacaoDAO : BaseDAO
    {

        /// <summary>
        /// Metodo construtor da classe Servidor Publicacao
        /// </summary>
        public ServidorPublicacaoDAO() : base()
        {
            CriaTabela();
        }

        /// <summary>
        /// Cria a tabela
        /// </summary>
        public void CriaTabela()
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS tba_servidor_publicacao (" +
                        "int_idfservidor INTEGER, " +
                        "int_idfpublicacao INTEGER, " +
                        "int_idftipoato INTEGER, " +
                        "int_paragrafoservidor INTEGER, " +
                        "int_paragrafotipoato INTEGER, " +
                        "vhr_nomecargo VARCHAR(100), " +
                        "vhr_codigocargo VARCHAR(20), " +
                        "vhr_tipoprovimento VARCHAR(20), " +
                        "vhr_orgaolotacao VARCHAR(200), " +
                        "FOREIGN KEY (int_idfservidor) REFERENCES tbl_servidor(int_idaservidor) ON UPDATE CASCADE ON DELETE CASCADE, " +
                        "FOREIGN KEY (int_idfpublicacao) REFERENCES tbl_publicacao(int_idapublicacao) ON UPDATE CASCADE ON DELETE CASCADE)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_servpub_servidor_asc ON tba_servidor_publicacao (int_idfservidor ASC)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_servpub_publicacao_asc ON tba_servidor_publicacao (int_idfpublicacao ASC)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recupera registro de associacao pelo id do servidor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ServidorPublicacao> GetServidor(int id)
        {
            List<ServidorPublicacao> retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT t1.*," +
                    "t2.vhr_nome as vhr_nomeservidor " +
                    "FROM tba_servidor_publicacao t1 " +
                    "INNER JOIN tbl_servidor t2 ON t1.int_idfservidor = t2.int_idaservidor " +
                    "WHERE t1.int_idfservidor = " + id;
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new List<ServidorPublicacao>();
                    foreach (DataRow linha in dataTable.Rows)
                    {
                        retorno.Add(new ServidorPublicacao(linha));
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Recupera registro de associacao pelos ids do servidor e publicacao
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServidorPublicacao Get(int idservidor, int idpublicacao)
        {
            ServidorPublicacao retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT t1.*," +
                    "t2.vhr_nome as vhr_nomeservidor " +
                    "FROM tba_servidor_publicacao t1 " +
                    "INNER JOIN tbl_servidor t2 ON t1.int_idfservidor = t2.int_idaservidor " +
                    "WHERE t1.int_idfservidor = " + idservidor + " " +
                    "AND t1.int_idfpublicacao = " + idpublicacao;
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new ServidorPublicacao(dataTable.Rows[0]);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Insere registro
        /// </summary>
        /// <param name="servidor"></param>
        public ServidorPublicacao Add(ServidorPublicacao servidorpub)
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO tba_servidor_publicacao (int_idfservidor, " +
                        "int_idfpublicacao, " +
                        "int_idftipoato, " +
                        "int_paragrafoservidor, " +
                        "int_paragrafotipoato," +
                        "vhr_nomecargo, " +
                        "vhr_codigocargo, " +
                        "vhr_tipoprovimento, " +
                        "vhr_orgaolotacao) " +
                        "VALUES (@idservidor, " +
                        "@idpublicacao, " +
                        "@idtipoato, " +
                        "@paragrafoservidor, " +
                        "@paragrafotipoato, " +
                        "@nomecargo, " +
                        "@codigocargo, " +
                        "@tipoprovimento, " +
                        "@orgaolotacao)";
                    cmd.Parameters.AddWithValue("@idservidor", servidorpub.IdServidor);
                    cmd.Parameters.AddWithValue("@idpublicacao", servidorpub.IdPublicacao);
                    cmd.Parameters.AddWithValue("@idtipoato", servidorpub.IdTipoAto);
                    cmd.Parameters.AddWithValue("@paragrafoservidor", servidorpub.ParagrafoServidor);
                    cmd.Parameters.AddWithValue("@paragrafotipoato", servidorpub.ParagrafoTipoAto);
                    cmd.Parameters.AddWithValue("@nomecargo", servidorpub.NomeCargo);
                    cmd.Parameters.AddWithValue("@codigocargo", servidorpub.CodigoCargo);
                    cmd.Parameters.AddWithValue("@tipoprovimento", servidorpub.TipoProvimento);
                    cmd.Parameters.AddWithValue("@orgaolotacao", servidorpub.OrgaoLotacao);
                    cmd.ExecuteNonQuery();
                }

                return Get(servidorpub.IdServidor, servidorpub.IdPublicacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
