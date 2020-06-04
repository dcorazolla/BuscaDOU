using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace BuscaDOU
{
    public class Navegador : Acao
    {

        public Navegador(Form1 form) : base(form)
        {

        }

        public void Log(string texto)
        {
            LogAcao("'Navegador' - " + texto);
        }

        /// <summary>
        /// Transforma string em documento HTML utilizando control webbrower
        /// </summary>
        /// <param name="html">String com HTML</param>
        /// <returns>Documento HTML</returns>
        public HtmlDocument GetHtmlDocument(string html)
        {
            HtmlDocument documento = null;
            try
            {
                //Log("Transformando dados para análise");
                WebBrowser browser = new WebBrowser
                {
                    ScriptErrorsSuppressed = true,
                    DocumentText = html
                };
                browser.Document.OpenNew(true);
                browser.Document.Write(html);
                browser.Refresh();
                documento = browser.Document;
                browser.Dispose();
                browser = null;
            }
            catch (Exception e)
            {
                Log("Ocorreu um erro ao tentar transformar os dados.");
                Log(e.Message);
            }
            return documento;
        }

        /// <summary>
        /// Realiza uma requisição http e retorna HtmlDocument
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HtmlDocument Navegar(String url)
        {
            HtmlDocument retorno = null;
            try
            {
                Log("Estabelecendo conexão URL: " + url);
                Uri uri = new Uri(url);
                WebRequest http = HttpWebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                StreamReader stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                String txtretorno = stream.ReadToEnd();
                response.Close();
                stream.Close();
                Log("Dados recebidos " + txtretorno.Length.ToString() + " bytes");
                retorno = GetHtmlDocument(txtretorno);
            }
            catch (UriFormatException e)
            {
                Log("URL invalida");
                Log(e.Message);
            }
            catch (IOException e)
            {
                Log("Não foi possível conectar");
                Log(e.Message);
            }

            return retorno;
        }

    }
}
