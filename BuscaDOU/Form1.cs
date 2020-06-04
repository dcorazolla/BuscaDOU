using BuscaDOU.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BuscaDOU
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// Valor inserido no campo de busca
        /// </summary>
        public String txtBusca;

        public BuscaDOU buscaDOU;

        public AtualizaDados atualizaDados;

        public Hashtable controlHashtable;

        public Thread thDOU;

        public Thread thDados;

        public int totalPublicaoes = 0;

        public bool atualizaServidor = true;

        public bool atualizaOrgao = true;

        public System.Windows.Forms.Timer timerAtualizaDados;

        public bool atualizacaoDadosRodando = false;

        public Form frmDetalhePublicacao;

        public Form1()
        {
            InitializeComponent();

           


            txtBusca = "";
            timerAtualizaDados = new System.Windows.Forms.Timer();
            timerAtualizaDados.Interval = 3000;
            timerAtualizaDados.Tick += TimerAtualizaDados_Tick;
            timerAtualizaDados.Start();

            dgv1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.Dgv1_RowsAdded);
            dgvDadosServidores.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DgvDadosServidores_RowsAdded);
            txtLog.TextChanged += new System.EventHandler(TxtBoxx_TextChanged);

            controlHashtable = new Hashtable();
            controlHashtable.Add("txtLog", txtLog);
            controlHashtable.Add("txtLogDOU", txtLogDOU);
            controlHashtable.Add("dgv1", dgv1);
            controlHashtable.Add("txtNome", txtNome);
            controlHashtable.Add("btnIniciar", btnIniciar);
            controlHashtable.Add("lblTotalPublicacoes", lblTotalPublicacoes);
            controlHashtable.Add("dgvDadosServidores", dgvDadosServidores);
            controlHashtable.Add("dgvDadosPublicacoes", dgvDadosPublicacoes);
            controlHashtable.Add("atualizacaoDadosRodando", atualizacaoDadosRodando);

            buscaDOU = new BuscaDOU(this);
            atualizaDados = new AtualizaDados(this);
            
        }

        private void DgvDadosServidores_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int totalDadosServidores = dgvDadosServidores.Rows.Count;
            lblDadosTotalServidores.Text = totalDadosServidores.ToString();
        }

        private void TimerAtualizaDados_Tick(object sender, EventArgs e)
        {
            if (!atualizacaoDadosRodando)
            {
                thDados = new Thread(atualizaDados.Inicia);
                //thDados.SetApartmentState(ApartmentState.STA);
                thDados.Start();
            }
            //throw new NotImplementedException();
        }

        private void TxtBoxx_TextChanged(object sender, EventArgs e)
        {
            ((TextBox)sender).SelectionStart = ((TextBox)sender).TextLength;
            ((TextBox)sender).ScrollToCaret();
        }

        private void Dgv1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            totalPublicaoes = dgv1.Rows.Count;
            lblTotalPublicacoes.Text = totalPublicaoes.ToString();
        }

        public Control GetTxtLogDOU()
        {
            return this.txtLogDOU;
        }

        /// <summary>
        /// Dispara evento ao clicar no botao iniciar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            thDOU = new Thread(buscaDOU.IniciaBusca);
            thDOU.SetApartmentState(ApartmentState.STA);
            thDOU.Start();
        }

        

        private Publicacao PegaInfoPublicacaoLink(Publicacao pub)
        {



            return pub;
        }

        

        /// <summary>
        /// Escreve LOG na tela
        /// </summary>
        /// <param name="texto">Texto para LOG</param>
        public void Log(String texto)
        {
            String data = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString()
                + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + ":"
                + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + "."
                + DateTime.Now.Millisecond.ToString();

            this.txtLog.Text = this.txtLog.Text + data + " - " + texto + "\r\n";
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            //thDOU.Sleep();
        }

        private void btnDadosFiltrar_Click(object sender, EventArgs e)
        {
            int codServidor = 0;
            if (dgvDadosServidores.SelectedRows.Count > 0)
            {
                codServidor = int.Parse(dgvDadosServidores.SelectedRows[0].Cells[0].Value.ToString());
            }
            if (codServidor > 0)
            {
                thDados = new Thread(() => atualizaDados.FiltraPublicacoes(codServidor));
                //thDados.SetApartmentState(ApartmentState.STA);
                thDados.Start();
            }
        }

        private void btnDetalhesDOU_Click(object sender, EventArgs e)
        {
            int codPublicacao = 0;
            if (dgv1.SelectedRows.Count > 0)
            {
                codPublicacao = int.Parse(dgv1.SelectedRows[0].Cells[0].Value.ToString());
            }

            if (codPublicacao > 0)
            {
                frmDetalhePublicacao = new FormDetalhesPublicacao(codPublicacao);
                frmDetalhePublicacao.Owner = this;
                frmDetalhePublicacao.Show();
            }
        }

        private void btnDadosDetalhesPublicacao_Click(object sender, EventArgs e)
        {
            int codPublicacao = 0;
            if (dgvDadosPublicacoes.SelectedRows.Count > 0)
            {
                codPublicacao = int.Parse(dgvDadosPublicacoes.SelectedRows[0].Cells[0].Value.ToString());
            }

            if (codPublicacao > 0)
            {
                frmDetalhePublicacao = new FormDetalhesPublicacao(codPublicacao);
                frmDetalhePublicacao.Owner = this;
                frmDetalhePublicacao.Show();
            }
        }
    }
}
