using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaDOU.Model
{
    public class Publicacao
    {

        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int Edicao { get; set; }
        public int Secao { get; set; }
        public int Pagina { get; set; }
        public string Orgao { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string Assinatura { get; set; }
        public string CargoAssinatura { get; set; }
        public string Link { get; set; }
        public DateTime Criacao { get; set; }
        public List<ServidorPublicacao> servidores;

        public Publicacao() { }

        public Publicacao(System.Data.DataRow dr)
        {
            Id = int.Parse(dr["int_idapublicacao"].ToString());
            Data = DateTime.ParseExact(dr["dte_data"].ToString(), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Edicao = int.Parse(dr["int_edicao"].ToString());
            Secao = int.Parse(dr["int_secao"].ToString());
            Pagina = int.Parse(dr["int_pagina"].ToString());
            Orgao = dr["vhr_orgao"].ToString();
            Titulo = dr["vhr_titulo"].ToString();
            Conteudo = dr["txt_conteudo"].ToString();
            Assinatura = dr["vhr_assinatura"].ToString();
            CargoAssinatura = dr["vhr_cargoassinatura"].ToString();
            Link = dr["vhr_link"].ToString();
            Criacao = DateTime.ParseExact(dr["dte_criacao"].ToString(), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
