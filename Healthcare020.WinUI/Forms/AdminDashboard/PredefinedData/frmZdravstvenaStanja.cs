﻿using Healthcare020.WinUI.Forms.AbstractForms;
using Healthcare020.WinUI.Helpers.Dialogs;
using Healthcare020.WinUI.Properties;
using Healthcare020.WinUI.Services;
using HealthCare020.Core.Constants;
using HealthCare020.Core.Models;
using HealthCare020.Core.ResourceParameters;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Healthcare020.WinUI.Forms.AdminDashboard.PredefinedData
{
    public sealed partial class frmZdravstvenaStanja : DisplayDataForm<ZdravstvenoStanjeDto>
    {
        private static frmZdravstvenaStanja _instance;

        public static frmZdravstvenaStanja Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new frmZdravstvenaStanja();
                return _instance;
            }
        }

        private frmZdravstvenaStanja(ZdravstvenoStanjeDto zdravstvenoStanje = null)
        {
            var ID = new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = nameof(ZdravstvenoStanjeDto.Id),
                CellTemplate = new DataGridViewTextBoxCell()
            };

            var Opis = new DataGridViewTextBoxColumn
            {
                Name = "Opis",
                HeaderText = Resources.Description,
                ToolTipText = "Opis zdravstvenog stanja",
                DataPropertyName = nameof(ZdravstvenoStanjeDto.Opis),
                CellTemplate = new DataGridViewTextBoxCell()
            };

            var Brisanje = new DataGridViewButtonColumn
            {
                Name = Resources.DeleteVerb,
                HeaderText = Resources.DeleteVerb,
                ToolTipText = "Izbriši zdravstveno stanje",
                Text = Resources.DeleteIt,
                CellTemplate = new DataGridViewButtonCell { ToolTipText = "Izbriši zdravstveno stanje", UseColumnTextForButtonValue = true },
                DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.Transparent, SelectionBackColor = Color.Transparent }
            };

            AddColumnsToMainDgrv(new DataGridViewColumn[] { ID, Opis, Brisanje });
            _apiService = new APIService(Routes.ZdravstvenaStanjaRoute);
            Text = Resources.frmZdravstvenaStanja;
            ResourceParameters = new ZdravstvenoStanjeResourceParameters();

            InitializeComponent();
        }

        protected override async void dgrvMain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!(MainDgrv.CurrentRow?.DataBoundItem is ZdravstvenoStanjeDto zdravstvenoStanje))
                return;

            if (MainDgrv.Columns[e.ColumnIndex].Name == Resources.DeleteVerb)
            {
                var prompDialog = dlgPropmpt.ShowDialog();

                if (prompDialog?.DialogResult == DialogResult.OK)
                {
                    var result = await _apiService.Delete<ZdravstvenoStanjeDto>(zdravstvenoStanje.Id);

                    if (result.Succeeded)
                    {
                        _dataForDgrv.Remove(zdravstvenoStanje);
                        CurrentRowCount--;
                        dlgSuccess.ShowDialog();
                    }
                }
            }
        }

        protected override void btnNew_Click(object sender, EventArgs e)
        {
            frmNewZdravstvenoStanje.ShowDialog();
        }

        protected override async void txtSearch_Leave(object sender, EventArgs e)
        {
            base.txtSearch_Leave(sender, e);

            if (ResourceParameters is ZdravstvenoStanjeResourceParameters resParams && SearchText.ToLower() != resParams.Opis)
            {
                resParams.Opis = SearchText;
                await LoadData();
            }
        }

        protected override void dgrvMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (MainDgrv?.CurrentRow?.DataBoundItem is ZdravstvenoStanjeDto zdravstvenoStanje)
            {
                frmNewZdravstvenoStanje.ShowDialogWithData(zdravstvenoStanje);
            }
        }

        private void frmZdravstvenaStanja_Load(object sender, EventArgs e)
        {
            FormForBackButton = frmPredefinedDataMenu.Instance;
            base.DisplayDataForm_Load(sender, e);
        }
    }
}