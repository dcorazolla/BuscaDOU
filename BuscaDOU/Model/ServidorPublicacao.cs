using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaDOU.Model
{
    public class ServidorPublicacao
    {

        public int IdServidor { get; set; }
        public string NomeServidor { get; set; }
        public int IdPublicacao { get; set; }
        public int IdTipoAto { get; set; }
        public int ParagrafoServidor { get; set; }
        public int ParagrafoTipoAto { get; set; }
        public string NomeCargo { get; set; }
        public string CodigoCargo { get; set; }
        public string TipoProvimento { get; set; }
        public string OrgaoLotacao { get; set; }

        public ServidorPublicacao(System.Data.DataRow dr)
        {
            IdServidor = int.Parse(dr["int_idfservidor"].ToString());
            NomeServidor = dr["vhr_nomeservidor"].ToString();
            IdPublicacao = int.Parse(dr["int_idfpublicacao"].ToString());
            IdTipoAto = int.Parse(dr["int_idftipoato"].ToString());
            ParagrafoServidor = int.Parse(dr["int_paragrafoservidor"].ToString());
            ParagrafoTipoAto = int.Parse(dr["int_paragrafotipoato"].ToString());
            NomeCargo = dr["vhr_nomecargo"].ToString();
            CodigoCargo = dr["vhr_codigocargo"].ToString();
            TipoProvimento = dr["vhr_tipoprovimento"].ToString();
            OrgaoLotacao = dr["vhr_orgaolotacao"].ToString();
        }

        public ServidorPublicacao()
        {
        }
    }
}
