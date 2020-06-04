using System;

namespace BuscaDOU.Model
{
    public class Servidor
    {
        public Servidor(int id, String nome)
        {
            Id = id;
            Nome = nome;
        }

        public Servidor(String nome)
        {
            Nome = nome;
        }

        public Servidor (System.Data.DataRow dr)
        {
            Id = int.Parse(dr["int_idaservidor"].ToString());
            Nome = dr["vhr_nome"].ToString();
            CPF = dr["vhr_cpf"].ToString();
            MatriculaSIAPE = int.Parse(dr["int_siape"].ToString());
            MatriculaSIGEPE = int.Parse(dr["int_sigepe"].ToString());
            CodigoTransparencia = int.Parse(dr["int_transparencia"].ToString());
            if (dr["dte_atualizacaodou"].ToString() != "") AtualizacaoDOU = DateTime.ParseExact(dr["dte_atualizacaodou"].ToString(), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            if (dr["dte_atualizacaotransparencia"].ToString() != "") AtualizacaoTransparencia = DateTime.ParseExact(dr["dte_atualizacaotransparencia"].ToString(), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            if (dr["dte_criacao"].ToString() != "") Criacao = DateTime.ParseExact(dr["dte_criacao"].ToString(), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

        public Servidor() { }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public int MatriculaSIAPE { get; set; }
        public int MatriculaSIGEPE { get; set; }
        public int CodigoTransparencia { get; set; }
        public DateTime AtualizacaoDOU { get; set; }
        public DateTime AtualizacaoTransparencia { get; set; }
        public DateTime Criacao { get; set; }
    }
}
