using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaDOU.Model
{
    class OrgaoDAO : BaseDAO
    {

        public OrgaoDAO() : base()
        {
            CriaTabela();
        }

        public void CriaTabela()
        {
            try
            {
                using (var cmd = Connect().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS tbl_orgao (" +
                        "int_idaorgao INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "vhr_nome VARCHAR(150), " +
                        "int_siape INTEGER, " +
                        "int_sigepe INTEGER, " +
                        "int_sigepe INTEGER)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_orgao_nome_asc ON tbl_orgao (vhr_nome ASC)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_orgao_siape_asc ON tbl_orgao (int_siape ASC)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE INDEX IF NOT EXISTS ix_orgao_sigepe_asc ON tbl_orgao (int_sigepe ASC)";
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
