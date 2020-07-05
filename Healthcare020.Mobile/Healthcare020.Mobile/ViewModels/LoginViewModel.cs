﻿using System.Windows.Input;
using Healthcare020.Mobile.Resources;
using Healthcare020.Mobile.Services;
using Xamarin.Forms;

namespace Healthcare020.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            LoginCommand= new Command(Login);
        }
        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        public ICommand LoginCommand { get; set; }

        private async void Login()
        {
            var loggedIn=await Auth.AuthenticateWithPassword(Username, Password);
            await Application.Current.MainPage.DisplayAlert("Log In",
                loggedIn ? "Uspesno logovani" : AppResources.InvalidLoginCredentials, "Ok");
        }
    }
}