﻿using System;
using System.Linq;
using Healthcare020.WinUI.Forms.AbstractForms;
using Healthcare020.WinUI.Services;
using HealthCare020.Core.Constants;
using HealthCare020.Core.Models;
using HealthCare020.Core.ResourceParameters;
using System.Windows.Forms;

namespace Healthcare020.WinUI.Forms.RadnikDashboard.DoktorDashboard
{
    public partial class frmDoktorZahteviZaPregledeDisplay : DisplayDataForm<ZahtevZaPregledDtoEL>
    {
        private static frmDoktorZahteviZaPregledeDisplay _instance = null;

        public static frmDoktorZahteviZaPregledeDisplay Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new frmDoktorZahteviZaPregledeDisplay();
                return _instance;
            }
        }

        private frmDoktorZahteviZaPregledeDisplay()
        {
            var ID = new DataGridViewTextBoxColumn { DataPropertyName = nameof(ZahtevZaPregledDtoEL.Id), HeaderText = "ID", Name = "ID", CellTemplate = new DataGridViewTextBoxCell() };

            var Pacijent = new DataGridViewColumn { HeaderText = "Pacijent", Name = "Pacijent", CellTemplate = new DataGridViewTextBoxCell() };

            var DatumVreme = new DataGridViewColumn { DataPropertyName = nameof(ZahtevZaPregledDtoEL.DatumVreme), HeaderText = "Datum i vreme", Name = "Datum i vreme", CellTemplate = new DataGridViewTextBoxCell() };


            base.AddColumnsToMainDgrv(new[] { ID, Pacijent, DatumVreme });

            _apiService = new APIService(Routes.ZahteviZaPregledRoute);
            ResourceParameters = new ZahtevZaPregledResourceParameters() { PageNumber = 1, PageSize = PossibleRowsCount,EagerLoaded = true};

            InitializeComponent();
        }

        protected override void dgrvMain_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];

            var pregled = row.DataBoundItem as ZahtevZaPregledDtoEL;

            if (pregled == null)
                return;

            if (dgrvMain.Columns[e.ColumnIndex].Name == "Pacijent")
            {
                e.Value = pregled.Pacijent.ZdravstvenaKnjizica.LicniPodaci.ImePrezime();
            }
        }

        protected override async void txtSearch_Leave(object sender, EventArgs e)
        {
            base.txtSearch_Leave(sender, e);
            var zahteviZaPregledResParams = ResourceParameters as ZahtevZaPregledResourceParameters;
            if (zahteviZaPregledResParams == null)
                return;

            var ShouldLoad = SearchText != zahteviZaPregledResParams.PacijentIme;

            if (ShouldLoad)
            {
                zahteviZaPregledResParams.PacijentIme = SearchText;
                zahteviZaPregledResParams.PacijentPrezime = SearchText;
                await base.LoadData();
            }
        }
    }
}