using System;
using System.Reflection;
using System.Windows.Forms;

namespace BuscaDOU
{
    public abstract class Acao
    {

        public Form1 form;

        public Acao(Form1 form)
        {
            this.form = form;
        }

        ~Acao()
        {
            //Application.ExitThread();
        }

        public virtual void Log(string texto)
        {
            LogAcao(texto);
        }

        /// <summary>
        /// Escreve LOG na tela
        /// </summary>
        /// <param name="texto">Texto para LOG</param>
        public void LogAcao(string texto)
        {
            // recupera controle txtLog do form principal
            TextBox txtLog = (TextBox)form.controlHashtable["txtLog"];
            // alterando texto do controle por reflection
            SetControlPropertyValue(txtLog, "Text", txtLog.Text + GetDataAtual() + " - " + texto + Environment.NewLine);
        }

        /// <summary>
        /// Retorna data atual
        /// </summary>
        /// <returns>DD/MM/YYYY HH:II:SS</returns>
        public string GetDataAtual()
        {
            return DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0')
                + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":"
                + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0') + "."
                + DateTime.Now.Millisecond.ToString().PadLeft(3, '0');
        }

        /// <summary>
        /// Delegate
        /// </summary>
        /// <param name="oControl">nome do controle</param>
        /// <param name="propName">nome da propriedade</param>
        /// <param name="propValue">valor</param>
        delegate void SetControlValueCallback(Control oControl, string propName, object propValue);

        /// <summary>
        /// Alterando valor da propriedadedo controle por reflection
        /// </summary>
        /// <param name="oControl">nome do controle</param>
        /// <param name="propName">nome da propriedade</param>
        /// <param name="propValue">valor</param>
        public void SetControlPropertyValue(Control oControl, string propName, object propValue)
        {
            if (oControl.InvokeRequired)
            {
                SetControlValueCallback d = new SetControlValueCallback(SetControlPropertyValue);
                oControl.Invoke(d, new object[] { oControl, propName, propValue });
            }
            else
            {
                Type t = oControl.GetType();
                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.Name.ToUpper() == propName.ToUpper())
                    {
                        p.SetValue(oControl, propValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// Delegate
        /// </summary>
        /// <param name="oControl">nome do controle</param>
        /// <param name="propName">nome da propriedade</param>
        /// <param name="propValue">valor</param>
        delegate void AdicionaLinhaCallback(DataGridView oControl, object[] linha);

        /// <summary>
        /// Alterando valor da propriedadedo controle por reflection
        /// </summary>
        /// <param name="oControl">nome do controle</param>
        /// <param name="propName">nome da propriedade</param>
        /// <param name="propValue">valor</param>
        public void AdicionaLinhaDGV(DataGridView oControl, object[] linha)
        {
            if (oControl.InvokeRequired)
            {
                AdicionaLinhaCallback d = new AdicionaLinhaCallback(AdicionaLinhaDGV);
                oControl.Invoke(d, new object[] { oControl, linha });
            }
            else
            {
                oControl.Rows.Add(linha);
            }
        }

        /// <summary>
        /// Delegate
        /// </summary>
        /// <param name="oControl">nome do controle</param>
        delegate void LimpaDGVCallback(DataGridView oControl);

        /// <summary>
        /// Alterando valor da propriedadedo controle por reflection
        /// </summary>
        /// <param name="oControl">nome do controle</param>
        public void LimpaDGV(DataGridView oControl)
        {
            if (oControl.InvokeRequired)
            {
                LimpaDGVCallback d = new LimpaDGVCallback(LimpaDGV);
                oControl.Invoke(d, new object[] { oControl });
            }
            else
            {
                oControl.Rows.Clear();
            }
        }

    }
}
