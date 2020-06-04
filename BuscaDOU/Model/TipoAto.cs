using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaDOU.Model
{
    public class TipoAto
    {

        public int Id { get; set; }
        public string Nome { get; set; }

        public TipoAto(System.Data.DataRow dr)
        {
            Id = int.Parse(dr["int_idatipoato"].ToString());
            Nome = dr["vhr_nome"].ToString();
        }

        public TipoAto(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }
    }
}
