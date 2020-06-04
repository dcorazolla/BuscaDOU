using BuscaDOU.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuscaDOU
{
    public partial class FormDetalhesPublicacao : Form
    {

        public int IdPublicacao = 0;
        private PublicacaoDAO publicacaoDAO;
        private Publicacao publicacao;

        public FormDetalhesPublicacao(int id)
        {
            InitializeComponent();

            IdPublicacao = id;
            publicacaoDAO = new PublicacaoDAO();
            publicacao = publicacaoDAO.Get(IdPublicacao);

            txtTitulo.Text = publicacao.Titulo;
            txtData.Text = publicacao.Data.ToShortDateString();
            txtSecao.Text = publicacao.Secao.ToString();
            txtEdicao.Text = publicacao.Edicao.ToString();
            txtPagina.Text = publicacao.Pagina.ToString();
            txtAssinatura.Text = publicacao.Assinatura;
            txtLink.Text = publicacao.Link;

            wbDetalhes.ScriptErrorsSuppressed = true;
            wbDetalhes.DocumentText = publicacao.Conteudo;
            wbDetalhes.Document.OpenNew(true);
            wbDetalhes.Document.Write(publicacao.Conteudo);
            wbDetalhes.Refresh();
            //WebBrowser browser = new WebBrowser
            //{
            //    ScriptErrorsSuppressed = true,
            //    DocumentText = html
            //};
            //browser.Document.OpenNew(true);
            //browser.Document.Write(html);
            //browser.Refresh();
        }
    }
}
