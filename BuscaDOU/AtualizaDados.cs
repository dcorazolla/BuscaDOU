using BuscaDOU.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuscaDOU
{
    public class AtualizaDados : Acao
    {

        private ServidorDAO servidorDAO;
        private ServidorPublicacaoDAO servidorPublicacaoDAO;
        private PublicacaoDAO publicacaoDAO;
        private TipoAtoDAO tipoAtoDAO;

        public AtualizaDados(Form1 frm) : base(frm)
        {
            servidorDAO = new ServidorDAO();
            servidorPublicacaoDAO = new ServidorPublicacaoDAO();
            publicacaoDAO = new PublicacaoDAO();
            tipoAtoDAO = new TipoAtoDAO();
        }

        public void Inicia()
        {
            if (!form.atualizacaoDadosRodando
                && (form.atualizaServidor))
            {
                Log("Iniciando atualização", 1);
                form.atualizacaoDadosRodando = true;

                if (form.atualizaServidor)
                {
                    Log("Atualizando dados de servidores", 1);
                    LimpaDGV((DataGridView)form.controlHashtable["dgvDadosServidores"]);
                    List<Servidor> servidores = servidorDAO.GetAll();
                    if (servidores.Count > 0)
                    {
                        foreach (Servidor serv in servidores)
                        {
                            AdicionaLinhaDGV((DataGridView)form.controlHashtable["dgvDadosServidores"], new object[] { serv.Id, serv.Nome });
                        }
                    }

                    form.atualizaServidor = false;
                }

                Thread.Sleep(10000);
                form.atualizacaoDadosRodando = false;
            }
        }

        public void FiltraPublicacoes(int codServidor)
        {
            LimpaDGV((DataGridView)form.controlHashtable["dgvDadosPublicacoes"]);
            List<ServidorPublicacao> servidoresPublicacao = servidorPublicacaoDAO.GetServidor(codServidor);
            if (servidoresPublicacao != null)
            {
                foreach (ServidorPublicacao servpub in servidoresPublicacao)
                {
                    Publicacao pub = publicacaoDAO.Get(servpub.IdPublicacao);
                    TipoAto tipo = tipoAtoDAO.Get(servpub.IdTipoAto);
                    string tipoNome = "";
                    if (tipo != null) tipoNome = tipo.Nome;

                    AdicionaLinhaDGV((DataGridView)form.controlHashtable["dgvDadosPublicacoes"], 
                        new object[] { pub.Id, tipoNome, pub.Titulo,
                        pub.Data.ToShortDateString(), pub.Edicao, pub.Secao,
                        pub.Pagina, pub.Orgao, pub.Assinatura,
                        pub.CargoAssinatura, 
                        pub.Link, pub.Conteudo, pub.Criacao});
                }
            }
        }

        /// <summary>
        /// Sobrescrita do metodo Log
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="nivel"></param>
        public void Log(string texto, int nivel)
        {
            if (nivel == 1)
            {
                //TextBox txtLogDOU = (TextBox)form.controlHashtable["txtLogDOU"];
                //SetControlPropertyValue(txtLogDOU, "Text", texto);
            }
            LogAcao("'Atualiza Dados' - " + texto);
        }

    }
}
