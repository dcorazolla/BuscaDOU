using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaDOU.Model
{
    public class TipoAtoDAO : BaseDAO
    {

        public enum Tipos { Nomeacao=1, Designacao, Exoneracao, Viagem, Nao_Identificado, Assinatura, Reconducao, Dispensa, Concessao, Requisicao };

        public TipoAtoDAO() : base()
        {
            CriaTabela();
        }

        /// <summary>
        /// Cria a tabela tipo ato
        /// </summary>
        public void CriaTabela()
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS tbl_tipoato (int_idatipoato INTEGER PRIMARY KEY, vhr_nome VARCHAR(150))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_tipoato_nome_asc ON tbl_tipoato (vhr_nome ASC)";
                    cmd.ExecuteNonQuery();
                }

                // inserindo tipos de atos
                if (this.Get("Nomeação") == null) this.Add(new TipoAto(1, "Nomeação"));
                if (this.Get("Designação") == null) this.Add(new TipoAto(2, "Designação"));
                if (this.Get("Exoneração") == null) this.Add(new TipoAto(3, "Exoneração"));
                if (this.Get("Viagem") == null) this.Add(new TipoAto(4, "Viagem"));
                if (this.Get("Não Identificado") == null) this.Add(new TipoAto(5, "Não Identificado"));
                if (this.Get("Assinatura") == null) this.Add(new TipoAto(6, "Assinatura"));
                if (this.Get("Recondução") == null) this.Add(new TipoAto(7, "Recondução"));
                if (this.Get("Dispensa") == null) this.Add(new TipoAto(8, "Dispensa"));
                if (this.Get("Concessão") == null) this.Add(new TipoAto(9, "Concessão"));
                if (this.Get("Requisição") == null) this.Add(new TipoAto(10, "Requisição"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recupera registro de tipo ato pelo nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public TipoAto Get(String nome)
        {
            TipoAto retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT * FROM tbl_tipoato WHERE vhr_nome = '" + nome + "'";
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new TipoAto(dataTable.Rows[0]);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Recupera registro de tipo ato pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TipoAto Get(int id)
        {
            TipoAto retorno = null;

            using (var comm = new System.Data.SQLite.SQLiteCommand(sqliteConnection))
            {
                comm.CommandText = "SELECT * FROM tbl_tipoato WHERE int_idatipoato = '" + id + "'";
                var adapter = new System.Data.SQLite.SQLiteDataAdapter(comm);
                var dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    retorno = new TipoAto(dataTable.Rows[0]);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Insere registro de tipo ato
        /// </summary>
        /// <param name="tipo"></param>
        public void Add(TipoAto tipo)
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO tbl_tipoato (int_idatipoato, vhr_nome) values (@id, @nome)";
                    cmd.Parameters.AddWithValue("@id", tipo.Id);
                    cmd.Parameters.AddWithValue("@nome", tipo.Nome);
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
