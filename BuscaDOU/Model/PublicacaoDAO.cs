using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaDOU.Model
{
    public class PublicacaoDAO : BaseDAO
    {

        /// <summary>
        /// Metodo construtor da classe PublicacaoDAO
        /// </summary>
        public PublicacaoDAO() : base()
        {
            CriaTabela();
        }

        /// <summary>
        /// Cria a tabela publicacao
        /// </summary>
        public void CriaTabela()
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS tbl_publicacao (" +
                        "int_idapublicacao INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "dte_data DATE, " +
                        "int_edicao INTEGER, " +
                        "int_secao INTEGER, " +
                        "int_pagina INTEGER, " +
                        "vhr_orgao VARCHAR(100), " +
                        "vhr_titulo VARCHAR (100), " +
                        "txt_conteudo TEXT, " +
                        "vhr_assinatura VARCHAR (100), " +
                        "vhr_cargoassinatura VARCHAR (150), " +
                        "vhr_link VARCHAR(200), " +
                        "dte_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_publicacao_link_asc ON tbl_publicacao (vhr_link ASC)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_publicacao_criacao_asc ON tbl_publicacao (dte_criacao ASC)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recupera registro de publicacao pelo link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public Publicacao Get(String link)
        {

            Publicacao retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT * FROM tbl_publicacao WHERE vhr_link = '" + link + "'";
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new Publicacao(dataTable.Rows[0]);
                }
            }

            return retorno;

        }

        /// <summary>
        /// Recupera registro de publicacao pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Publicacao Get(int id)
        {
            Publicacao retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT * FROM tbl_publicacao WHERE int_idapublicacao = " + id.ToString();
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new Publicacao(dataTable.Rows[0]);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Insere ou atualiza registro de publicacao
        /// </summary>
        /// <param name="publicacao"></param>
        public Publicacao Save(Publicacao publicacao)
        {
            if (publicacao.Id == 0)
            {
                return Add(publicacao);
            }
            else
            {
                return Update(publicacao);
            }
        }

        /// <summary>
        /// Insere registro de publicacao
        /// </summary>
        /// <param name="publicacao"></param>
        public Publicacao Add(Publicacao publicacao)
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO tbl_publicacao " +
                        "(dte_data, " +
                        "int_edicao, " +
                        "int_secao, " +
                        "int_pagina, " +
                        "vhr_orgao, " +
                        "vhr_titulo, " +
                        "txt_conteudo, " +
                        "vhr_assinatura, " +
                        "vhr_cargoassinatura, " +
                        "vhr_link) " +
                        "values " +
                        "(@data, " +
                        "@edicao, " +
                        "@secao, " +
                        "@pagina, " +
                        "@orgao, " +
                        "@titulo, " +
                        "@conteudo, " +
                        "@assinatura, " +
                        "@cargoassinatura, " +
                        "@link)";
                    cmd.Parameters.AddWithValue("@data", publicacao.Data);
                    cmd.Parameters.AddWithValue("@edicao", publicacao.Edicao);
                    cmd.Parameters.AddWithValue("@secao", publicacao.Secao);
                    cmd.Parameters.AddWithValue("@pagina", publicacao.Pagina);
                    cmd.Parameters.AddWithValue("@orgao", publicacao.Orgao);
                    cmd.Parameters.AddWithValue("@titulo", publicacao.Titulo);
                    cmd.Parameters.AddWithValue("@conteudo", publicacao.Conteudo);
                    cmd.Parameters.AddWithValue("@assinatura", publicacao.Assinatura);
                    cmd.Parameters.AddWithValue("@cargoassinatura", publicacao.CargoAssinatura);
                    cmd.Parameters.AddWithValue("@link", publicacao.Link);
                    cmd.ExecuteNonQuery();
                }

                return Get(publicacao.Link);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Atualiza registro de publicacao
        /// </summary>
        /// <param name="publicacao"></param>
        public Publicacao Update(Publicacao publicacao)
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    if (publicacao.Id != 0)
                    {
                        //cmd.CommandText = "UPDATE tbl_servidor SET " +
                        //    "vhr_nome = @nome " +
                        //    "WHERE int_idaservidor = @id";
                        //cmd.Parameters.AddWithValue("@id", servidor.Id);
                        //cmd.Parameters.AddWithValue("@nome", servidor.Nome);
                        //cmd.ExecuteNonQuery();
                    }

                    return Get(publicacao.Link);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Apaga registro de publicacao
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            try
            {
                using (var cmd = new SQLiteCommand(Connect()))
                {
                    //cmd.CommandText = "DELETE FROM tbl_servidor WHERE int_idaservidor = @id";
                    //cmd.Parameters.AddWithValue("@id", id);
                    //cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
