﻿using Healthcare020.Mobile.Controls;
using Healthcare020.Mobile.Interfaces;
using Healthcare020.Mobile.Resources;
using HealthCare020.Core.Constants;
using HealthCare020.Core.Extensions;
using HealthCare020.Core.Models;
using HealthCare020.Core.ResourceParameters;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using HealthCare020.Core.Request;
using Healthcare020.Mobile.Services;
using Healthcare020.Mobile.Views;
using Xamarin.Forms;

namespace Healthcare020.Mobile.ViewModels
{
    public class PosetaViewModel : BaseValidationViewModel
    {
        private readonly IAPIService _apiService;

        public string CurrentPacijentSearch;

        public PosetaViewModel(IAPIService apiService)
        {
            _apiService = apiService;
            _apiService.ChangeRoute(Routes.PacijentNaLecenjuRoute);
        }


        #region Commands

        public ICommand SearchPacijentiCommand => new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(PacijentSearch))
                return;

            IsBusy = true;
            CurrentPacijentSearch = PacijentSearch;

            var resParams = new PacijentNaLecenjuResourceParameters
            {
                EagerLoaded = true,
            };

            var imePrezime = PacijentSearch.Split(' ');
            if (imePrezime.Length == 2)
            {
                resParams.Ime = imePrezime[0];
                resParams.Prezime = imePrezime[1];
            }
            else if (imePrezime.Length == 1)
            {
                resParams.Ime = PacijentSearch;
                resParams.Prezime = PacijentSearch;
            }

            var result = await _apiService.Get<PacijentNaLecenjuDtoEL>(resParams);
            IsBusy = false;

            if (result.Succeeded && result.HasData)
            {
                Pacijenti = result.Data.ToObservableCollection();
                PickerIsReady = CustomPicker.PickerIsReady;
            }
        });

        public ICommand  CreatePosetaCommand => new Command(async () =>
        {
            IsBusy = true;
            _apiService.ChangeRoute(Routes.ZahtevZaPosetuRoute);

            var upsertDto = new ZahtevZaPosetuUpsertDto
            {
                PacijentNaLecenjuId = PacijentPicked.Id,
                BrojTelefonaPosetioca = BrojTelefonaPosetioca
            };

            var result = await _apiService.Post<ZahtevZaPosetuDtoLL>(upsertDto);
            IsBusy = false;

            if (result.Succeeded)
            {
                 NotificationService.Instance.Success("Uspesno prosledjen zahtev za posetu");
                 await Task.Delay(300);
                 Application.Current.MainPage=new WelcomePage();
            }
        });

        public ICommand CancelNavigationCommand => new Command(async () =>
        {
            if ((await NotificationService.Instance.Prompt()).Ok)
                Application.Current.MainPage = new WelcomePage();
        });

        #endregion Commands

        #region Properties

        private string _pickerIsReady;

        public string PickerIsReady
        {
            get => _pickerIsReady;
            set => SetProperty(ref _pickerIsReady, value);
        }

        private string _pacijentSearch;

        public string PacijentSearch
        {
            get => _pacijentSearch;
            set => SetProperty(ref _pacijentSearch, value);
        }

        private ObservableCollection<PacijentNaLecenjuDtoEL> _pacijenti;

        public ObservableCollection<PacijentNaLecenjuDtoEL> Pacijenti
        {
            get => _pacijenti;
            set => SetProperty(ref _pacijenti, value);
        }

        private PacijentNaLecenjuDtoEL _pacijentPicked;

        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.RequiredFieldError))]
        public PacijentNaLecenjuDtoEL PacijentPicked
        {
            get => _pacijentPicked;
            set
            {
                SetProperty(ref _pacijentPicked, value);
                if (_pacijentPicked != null)
                    PacijentSearch = _pacijentPicked.ImePrezime;
            }
        }

        private string _brojTelefonaPosetioca;

        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = nameof(AppResources.RequiredFieldError))]
        public string BrojTelefonaPosetioca
        {
            get => _brojTelefonaPosetioca;
            set
            {
                if (ValidateProperty(nameof(BrojTelefonaPosetioca), value))
                {
                    SetProperty(ref _brojTelefonaPosetioca, value);
                }
            }
        }

        #endregion Properties
    }
}