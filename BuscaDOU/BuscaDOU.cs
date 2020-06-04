using BuscaDOU.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BuscaDOU
{
    public class BuscaDOU : Acao
    {

        /// <summary>
        /// URL base da busca
        /// </summary>
        private string urlBase = "http://www.in.gov.br/consulta?";

        /// <summary>
        /// Total de registros por consulta.
        /// Recomendado ficar entre 10 e 50 para não onera site in.gov.br
        /// </summary>
        private int totalRegistros = 50;

        private ServidorDAO servidorDAO;
        private PublicacaoDAO publicacaoDAO;
        private TipoAtoDAO tipoAtoDAO;
        private ServidorPublicacaoDAO servidorPublicacaoDAO;
        private Navegador nav;
        public DataGridView dgv1;
        private TextBox txtNome;
        private List<Publicacao> publicacoes;
        private Label lblTotalPublicacoes;

        public int ParagrafoServidor = 0;
        public int ParagrafoTipoAto = 0;

        public int TotalRegistros { get => totalRegistros; set => totalRegistros = value; }
        public string UrlBase { get => urlBase; set => urlBase = value; }

        public BuscaDOU(Form1 form) : base(form)
        {
            Log("Iniciando componente 'Busca DOU'", 1);
            nav = new Navegador(form);
            dgv1 = (DataGridView)form.controlHashtable["dgv1"];
            txtNome = (TextBox)form.controlHashtable["txtNome"];
            servidorDAO = new ServidorDAO();
            publicacaoDAO = new PublicacaoDAO();
            tipoAtoDAO = new TipoAtoDAO();
            servidorPublicacaoDAO = new ServidorPublicacaoDAO();
            Log("Componente 'Busca DOU' iniciado", 1);
        }

        ~BuscaDOU()
        {
            //Application.ExitThread();
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
                TextBox txtLogDOU = (TextBox)form.controlHashtable["txtLogDOU"];
                SetControlPropertyValue(txtLogDOU, "Text", texto);
            }
            LogAcao("'Busca DOU' - " + texto);
        }

        /// <summary>
        /// Inicia busca
        /// </summary>
        /// <param name="nome"></param>
        public void IniciaBusca(string nome)
        {
            txtNome.Text = nome;
            IniciaBusca();
        }

        /// <summary>
        /// Verifica se existe servidor no banco, se não existir grava versão inicial do registro
        /// </summary>
        /// <param name="txtBusca"></param>
        /// <returns></returns>
        public Servidor VerificaExistenciaServidorBanco(string txtBusca)
        {
            // verifica se existe servidor no banco de dados
            Servidor servidor = servidorDAO.Get(txtBusca);
            // não encontrou servidor
            if (servidor == null)
            {
                // cria registro do servidor apenas com nome informado
                servidor = servidorDAO.Save(new Servidor(txtBusca));
                // informa a thread de dados que existe atualização de servidor pendente
                form.atualizaServidor = true;
            }

            return servidor;
        }

        /// <summary>
        /// Verifica existencia de publicação no banco pelo Link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public Publicacao VerificaExistenciaPublicacaoBanco(string link, HtmlDocument respostaPagina, int indice)
        {
            // verificando se existe publicacao no banco de dados
            Publicacao publicacao = publicacaoDAO.Get(link);
            // se nao existir grava
            if (publicacao == null)
            {
                // envia log para usuário
                Log("Acessando '" + link + "'", 1);
                // pegando informações completas da publicação
                publicacao = PegaInfoPublicacao(respostaPagina, indice, true);
                // salva publicacao
                publicacao = publicacaoDAO.Save(publicacao);
            }
            else
            {
                Log("Carregando '" + link + "' do banco", 1);
            }
            // retorna objeto Publicacao
            return publicacao;
        }

        public ServidorPublicacao IdentificaRelacaoAtoServidor(Publicacao publicacao, Servidor servidor)
        {
            Log("Identificando relação do servidor com a publicação", 1);

            ServidorPublicacao servpub = new ServidorPublicacao();
            servpub.IdServidor = servidor.Id;
            servpub.IdPublicacao = publicacao.Id;
            servpub.IdTipoAto = 0;
            servpub.ParagrafoServidor = 0;
            servpub.ParagrafoTipoAto = 0;
            servpub.NomeCargo = "";
            servpub.OrgaoLotacao = "";
            servpub.TipoProvimento = "";
            servpub.CodigoCargo = "";
            
            HtmlDocument conteudo = nav.GetHtmlDocument(publicacao.Conteudo);

            HtmlElementCollection ps = conteudo.GetElementsByTagName("p");
            int retorno = (int)TipoAtoDAO.Tipos.Nao_Identificado;

            // procurando paragrafo com nome do servidor
            for (int contver = 0; contver < ps.Count; contver++)
            {

                // paragrafo encontrado
                if (ps[contver].InnerText != null
                    && ps[contver].InnerText.IndexOf(servidor.Nome, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    // guardando indice do paragrafo
                    servpub.ParagrafoServidor = contver;
                    servpub = VerificaTipo(servpub, contver, conteudo);

                    int posicaoCPF = ps[contver].InnerText.IndexOf("CPF", StringComparison.CurrentCultureIgnoreCase);
                    int posicaoSiape = ps[contver].InnerText.IndexOf("SIAPE", StringComparison.CurrentCultureIgnoreCase);
                    int posicaoCargo = -1;

                    string[] termosCargo = { "cargo em comissão de", "cargo em comissão", "cargo de", "cargo" };
                    foreach (string termo in termosCargo)
                    {
                        posicaoCargo = ps[contver].InnerText.IndexOf(termo, StringComparison.CurrentCultureIgnoreCase);
                        if (posicaoCargo != -1)
                        {
                            int posicaoVirgula = ps[contver].InnerText.IndexOf(",", posicaoCargo + termo.Length);
                            if (posicaoVirgula == -1) { posicaoVirgula = ps[contver].InnerText.Substring(posicaoCargo + termo.Length).Length; }
                            servpub.NomeCargo = ps[contver].InnerText.Substring(posicaoCargo + termo.Length, posicaoVirgula - (posicaoCargo + termo.Length)).Trim();
                            break;
                        }
                    }

                    //int posicaoCargo = ps[contver].InnerText.IndexOf("cargo em comissão", StringComparison.CurrentCultureIgnoreCase);
                    //if (posicaoCargo != -1) posicaoCargo = posicaoCargo + 17;
                    //if (posicaoCargo == -1) posicaoCargo = ps[contver].InnerText.IndexOf("cargo", StringComparison.CurrentCultureIgnoreCase);

                    // se encontrar termo CPF no paragrafo com o nome do servidor, pega valor e guarda no banco
                    if ((servidor.CPF == null || servidor.CPF == "" || Regex.Match(servidor.CPF, @"[\*]{3}.\d{3}\.\d{3}-[\*]{2}").Success) && posicaoCPF != -1)
                    {
                        string temp = ps[contver].InnerText.Substring(posicaoCPF + 3, 20);
                        Match match = null;
                        match = Regex.Match(temp, @"[\d]{3}.\d{3}\.\d{3}-[\d]{2}");
                        if (!match.Success) match = Regex.Match(temp, @"[\*\d]{3}.\d{3}\.\d{3}-[\*\d]{2}");
                        if (!match.Success) match = Regex.Match(temp, @"\d{3}\.\d{3}");

                        if (match.Success) temp = match.Value;
                        if (temp.Length == 7) temp = "***." + temp + "-**";
                        servidor.CPF = temp;
                        servidor = servidorDAO.Save(servidor);
                    }

                    // se encontrar termo SIAPE no paragrafo com o nome do servidor, pega valor e guarda no banco
                    if ((servidor.MatriculaSIAPE == 0 /*|| Regex.Match(servidor.MatriculaSIAPE, @"[\*]{3}.\d{3}\.\d{3}-[\*]{2}").Success*/) && posicaoSiape != -1)
                    {
                        string temp = ps[contver].InnerText.Substring(posicaoSiape + 5, 20);
                        Match match = null;
                        match = Regex.Match(temp, @"[\d]{7}");
                        //if (!match.Success) match = Regex.Match(temp, @"[\*\d]{3}.\d{3}\.\d{3}-[\*\d]{2}");
                        //if (!match.Success) match = Regex.Match(temp, @"\d{3}\.\d{3}");

                        if (match.Success) temp = match.Value;
                        //if (temp.Length == 7) temp = "***." + temp + "-**";
                        servidor.MatriculaSIAPE = int.Parse(temp);
                        servidor = servidorDAO.Save(servidor);
                    }
                }
            }
            return servpub;
        }

        /// <summary>
        /// Realiza várias requisições, conforme número de páginas de resposta
        /// </summary>
        /// <param name="paginas"></param>
        /// <param name="txtBusca"></param>
        /// <param name="total"></param>
        public void buscaPaginasResultados(int paginas, string txtBusca, int total, Servidor servidor)
        {
            // percorrendo páginas de resultados
            for (int i = 1; i <= paginas; i++)
            {
                // captura html de resposta
                HtmlDocument respostaPagina = nav.Navegar(MontaUrlBusca(txtBusca, totalRegistros, i));
                // precorrendo página de resposta
                for (int j = 1; j <= totalRegistros; j++)
                {
                    // deifnindo numero do registro
                    int numero = j * i;
                    // a partir da segunda página de resultados
                    if (i > 1) numero = ((i - 1) * totalRegistros) + j;
                    // verifica se 
                    if (numero <= total)
                    {
                        // pegando informações iniciais da publicação
                        Publicacao pub = PegaInfoPublicacao(respostaPagina, j, false);
                        // publicação existe no banco?
                        Publicacao publicacao = VerificaExistenciaPublicacaoBanco(pub.Link, respostaPagina, j);

                        // Associando registros de servidor com publicação
                        ServidorPublicacao servpub = servidorPublicacaoDAO.Get(servidor.Id, publicacao.Id);
                        if (servpub == null)
                        {
                            servpub = IdentificaRelacaoAtoServidor(publicacao, servidor);
                            servpub = servidorPublicacaoDAO.Add(servpub);
                        }

                        // Adiciona linha no GRID
                        AdicionaLinhaDGV(dgv1, new object[] { publicacao.Id, publicacao.Titulo,
                                                publicacao.Data.ToShortDateString(), publicacao.Edicao, publicacao.Secao,
                                                publicacao.Pagina, publicacao.Orgao, publicacao.Assinatura,
                                                publicacao.CargoAssinatura, publicacao.Link, publicacao.Conteudo,
                                                publicacao.Criacao});

                    }
                }
            }
        }

        /// <summary>
        /// Inicia busca
        /// </summary>
        public void IniciaBusca()
        {
            int total = 0;
            int paginas = 1;
            string txtBusca = txtNome.Text;
            string txtValorBusca = txtBusca;

            // quebra o valor do campo de busca em array de strings para busca multipla
            string[] nomes = txtBusca.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            // rodando array de nomes para aplicar correções
            for (int i=0; i < nomes.Length; i++ )
            {
                // removendo espaços anteriores e posteriores do nome e transformando em caixa alta
                nomes[i] = nomes[i].Trim().ToUpper();
            }

            // se nao tiver preenchido o campo de busca
            if (txtBusca == "")
            {
                // solicita preenchimento
                Log("Preencha o nome", 1);
            }
            else
            {
                try
                {
                    // desativando campo de nomes
                    SetControlPropertyValue(txtNome, "Enabled", false);
                    // desativando botão de busca, para evitar multiplos threads
                    SetControlPropertyValue((Button)form.controlHashtable["btnIniciar"], "Enabled", false);
                    // percorre array de nomes para realização da busca
                    for (int contnomes = 0; contnomes < nomes.Length; contnomes++)
                    {
                        txtBusca = nomes[contnomes];
                        Log("Iniciando busca por '" + txtBusca + "'", 1);
                        // limpa datagridview da lista de publicações encontradas
                        LimpaDGV(dgv1);
                        // zera contador do datagridview
                        SetControlPropertyValue((Label)form.controlHashtable["lblTotalPublicacoes"], "Text", "0");
                        // verifica registro no banco de dados
                        Servidor servidor = VerificaExistenciaServidorBanco(txtBusca);
                        // grava data e hora da atualização da informações do DOU
                        servidor.AtualizacaoDOU = DateTime.Now;
                        // atualiza dados do servidor
                        servidor = servidorDAO.Save(servidor);
                        // realiza busca no site da imprensa nacional
                        HtmlDocument respostaBusca = nav.Navegar(MontaUrlBusca(txtBusca, 1, 1));
                        // caso tenha funcionado a requisição http
                        if (respostaBusca != null)
                        {
                            // identifica total de publicacoes
                            total = TotalPublicacoes(respostaBusca);
                            // envia log para usuário com total de publicações
                            Log(total.ToString() + " publicações encontradas para '" + txtBusca + "'", 1);
                            // se tiver alguma publicação nba resposta html, busca publicacoes
                            if (total > 0)
                            {
                                // calculando quantas requisições serão feitas
                                paginas = (int)Math.Ceiling((float)total / (float)totalRegistros);
                                // cria lista de publicações vazia
                                publicacoes = new List<Publicacao>();
                                // busca paginas de resposta
                                buscaPaginasResultados(paginas, txtBusca, total, servidor);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log("Ocorreu um erro. Olhe a aba de LOG.", 1);
                    Log(ex.Message, 0);
                }
                finally
                {
                    SetControlPropertyValue((TextBox)form.controlHashtable["txtNome"], "Enabled", true);
                    SetControlPropertyValue((Button)form.controlHashtable["btnIniciar"], "Enabled", true);
                }
            }
        }

        /// <summary>
        /// Verifica com expressao regular o tipo de ato
        /// </summary>
        /// <param name="indice"></param>
        /// <param name="pub"></param>
        /// <returns></returns>
        private ServidorPublicacao VerificaTipo(ServidorPublicacao servpub, int indice, HtmlDocument pub)
        {
            ParagrafoTipoAto = 0;
            HtmlElementCollection ps = pub.GetElementsByTagName("p");
            int tempindice = indice;
            int retorno = 0;

            Regex ERNomeacao = new Regex(@"nomea", RegexOptions.IgnoreCase);
            Regex ERDesignacao = new Regex(@"designa", RegexOptions.IgnoreCase);
            Regex ERExonera = new Regex(@"exonera", RegexOptions.IgnoreCase);
            Regex ERReconduz = new Regex(@"reconduz", RegexOptions.IgnoreCase);
            Regex ERDispensa = new Regex(@"dispensa", RegexOptions.IgnoreCase);
            Regex ERConcede = new Regex(@"concede", RegexOptions.IgnoreCase);
            Regex ERRequisita = new Regex(@"requisi", RegexOptions.IgnoreCase);

            if (indice == (ps.Count - 1))
            {
                servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Assinatura;
            }
            else
            {
                for (int i = tempindice; i >= 0; i--)
                {
                    if (ps[i].InnerText != null)
                    {
                        // verifica se é nomeacao
                        if (ERNomeacao.IsMatch(ps[i].InnerText)) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Nomeacao;
                        // designação
                        if (ERDesignacao.IsMatch(ps[i].InnerText)) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Designacao;
                        // exoneração
                        if (ERExonera.IsMatch(ps[i].InnerText)) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Exoneracao;
                        // recondução
                        if (ERReconduz.IsMatch(ps[i].InnerText)) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Reconducao;
                        // dispensa
                        if (ERDispensa.IsMatch(ps[i].InnerText)) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Dispensa;
                        // concessao
                        if (ERConcede.IsMatch(ps[i].InnerText)) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Concessao;
                        // requisicao
                        if (ERRequisita.IsMatch(ps[i].InnerText)) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Requisicao;
                    }
                    if (servpub.IdTipoAto != 0)
                    {
                        servpub.ParagrafoTipoAto = i;
                        break;
                    }
                }
            }

            if (servpub.IdTipoAto == 0) servpub.IdTipoAto = (int)TipoAtoDAO.Tipos.Nao_Identificado;

            return servpub;
        }


        /// <summary>
        /// monta URL de busca no DOU
        /// </summary>
        /// <param name="txt">Texto para busca</param>
        /// <param name="qtde">Quantidade de registros por pagina</param>
        /// <param name="pagina">Pagina da busca</param>
        /// <returns>Url montada</returns>
        private String MontaUrlBusca(String txt, int qtde, int pagina, string dataInicio)
        {
            String dataDe = dataInicio;
            DateTime valorParse;

            if (!DateTime.TryParse(dataDe, out valorParse)) dataDe = "1980-01-01";
            String dataAte = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0');

            return this.urlBase + "q=%22" + Uri.EscapeUriString(txt)
                + "%22&s=do2&publishFrom=" + dataDe
                + "&publishTo=" + dataAte
                + "&delta=" + qtde.ToString()
                + "&start=" + pagina.ToString();
        }

        private String MontaUrlBusca(String txt, int qtde, int pagina)
        {
            return MontaUrlBusca(txt, qtde, pagina, "1980-01-01");
        }

        /// <summary>
        /// Pega informação de total no HTML de resposta da busca
        /// </summary>
        /// <param name="html">HTML de resposta</param>
        /// <returns>Total de publicações encontradas</returns>
        private int TotalPublicacoes(HtmlDocument html)
        {
            int retorno = 0;
            this.Log("Verificando total de publicações");
            // quebrando o html pelos elementos p
            HtmlElementCollection elems = html.GetElementsByTagName("p");
            // percorrendo coleção de paragrafos
            foreach (HtmlElement elem in elems)
            {
                // verificando existencia de linha total
                if (elem.GetAttribute("classname").IndexOf("search-total-label") > -1)
                {
                    // expressao regular para extrair apenas números
                    Regex regex = new Regex(@"\d+");
                    // casa a expressão regular com o conteúdo string do elemento
                    Match match = regex.Match(elem.InnerText);
                    // se tiver casado converte em int e atribui a retorno
                    if (match.Success) retorno = int.Parse(match.Value);
                    // envia log interno com total de publicações
                    //Log("Total de publicações: " + retorno.ToString(), 0);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Busca infos da publicação, realizando requisição http
        /// </summary>
        /// <param name="indice"></param>
        /// <param name="pub"></param>
        /// <returns></returns>
        public Publicacao PegaInfoPublicacaoAvancado(int indice, Publicacao pub)
        {
            // requisitando url da publicacao
            HtmlDocument htmlPublicacao = nav.Navegar(pub.Link);
            if (htmlPublicacao != null)
            {
                Log("Coletando dados da publicação: " + pub.Titulo, 0);

                try
                {
                    HtmlElement detalhes = null;
                    HtmlElement texto = null;
                    // separando o caonteudo da resposta em divs, para detectar detalhes e texto da publicação
                    HtmlElementCollection divs = htmlPublicacao.GetElementsByTagName("div");
                    foreach(HtmlElement div in divs)
                    {
                        if (div.GetAttribute("classname").IndexOf("detalhes-dou") != -1) detalhes = div;
                        if (div.GetAttribute("classname").IndexOf("texto-dou") != -1) texto = div;
                        if (detalhes != null && texto != null) break;
                    }

                    if (detalhes!=null)
                    {
                        // dividindo bloco de detalhes nos spans
                        HtmlElementCollection spans = detalhes.GetElementsByTagName("span");
                        foreach (HtmlElement span in spans)
                        {
                            // data publicacao
                            if (span.GetAttribute("classname").IndexOf("publicado-dou-data") != -1)
                                pub.Data = DateTime.ParseExact(span.InnerText, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            // edicao
                            if (span.GetAttribute("classname").IndexOf("edicao-dou-data") != -1)
                                pub.Edicao = System.Convert.ToInt32(span.InnerText);

                            // secao
                            if (span.GetAttribute("classname").IndexOf("secao-dou") != -1)
                            {
                                Regex regex = new Regex(@"\d+");
                                Match match = regex.Match(span.InnerText);
                                if (match.Success) pub.Secao = System.Convert.ToInt32(match.Value);
                            }

                            // pagina
                            if (span.GetAttribute("classname").IndexOf("secao-dou-data") != -1)
                                pub.Pagina = System.Convert.ToInt32(span.InnerText);

                            // orgao
                            if (span.GetAttribute("classname").IndexOf("orgao-dou-data") != -1)
                                pub.Orgao = span.InnerText;
                        }
                        // liberando memoria
                        spans = null;
                    }

                    if (texto != null)
                    {
                        // dividindo o bloco de texto em paragrafos html
                        HtmlElementCollection ps = texto.GetElementsByTagName("p");
                        int cont = 0;
                        foreach (HtmlElement p in ps)
                        {
                            cont++;
                            if (p.InnerText != null)
                            {
                                // segundo paragrafo é onde normalmente encontramos o cargo de quem assina o documento
                                if ((pub.CargoAssinatura == "" || pub.CargoAssinatura == null) && (cont == 1 || cont == 2 || cont == 3 || cont == 4))
                                {
                                    string txtparte = p.InnerText;
                                    // posicao da virgula
                                    int indiceparte = txtparte.IndexOf(",");
                                    int indiceparte2 = txtparte.IndexOf("no uso");
                                    string txtparte2 = "";
                                    if (indiceparte != -1) txtparte2 = txtparte.Substring(0, indiceparte);
                                    else if (indiceparte2 != -1) txtparte2 = txtparte.Substring(0, indiceparte2);
                                    if (txtparte2 != "" && (txtparte2.Substring(0, 2).ToUpper() == "O " || txtparte2.Substring(0, 2).ToUpper() == "A ")) txtparte2 = txtparte2.Substring(2);
                                    if (txtparte2 != "" && (txtparte2.Substring(0, 1).ToUpper() == "O")) txtparte2 = txtparte2.Substring(1);
                                    if (txtparte2.IndexOf("portaria", StringComparison.CurrentCultureIgnoreCase) != -1) txtparte2 = "";
                                    pub.CargoAssinatura = txtparte2.ToUpper();
                                }

                                // assinatura
                                if (p.GetAttribute("classname").IndexOf("assina") != -1
                                    || p.GetAttribute("classname").IndexOf("pr") != -1)
                                    pub.Assinatura = p.InnerText.ToUpper();

                                if (p.GetAttribute("classname").IndexOf("cargo") != -1)
                                    pub.CargoAssinatura = p.InnerText.ToUpper();
                            }

                        }

                        pub.Conteudo = texto.InnerHtml.ToString();

                        ps = null;
                    }
                }
                catch (Exception e)
                {
                    this.Log("Ocorreu um erro ao coletar dados da publicacao #" + indice.ToString());
                    this.Log(e.Message);
                }
            }

            // zerando htmldocument para liberar memoria
            htmlPublicacao = null;

            return pub;
        }

        /// <summary>
        /// Pega informações da publicação 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="indice"></param>
        /// <param name="navega"></param>
        /// <returns></returns>
        public Publicacao PegaInfoPublicacao(HtmlDocument html, int indice, bool navega)
        {
            Publicacao pub = null;

            HtmlElementCollection h5s = html.GetElementsByTagName("h5");
            if (h5s.Count > 0 && (indice - 1) < h5s.Count)
            {
                pub = new Publicacao();
                pub.Titulo = h5s[indice - 1].InnerText;
                HtmlElementCollection links = h5s[indice - 1].GetElementsByTagName("a");
                pub.Link = links[0].GetAttribute("href");
                links = null;

                if (navega)
                {
                    pub = PegaInfoPublicacaoAvancado(indice, pub);
                }

            }

            return pub;
        }

    }
}
