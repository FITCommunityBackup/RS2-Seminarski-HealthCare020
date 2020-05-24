﻿using System.Net;

namespace Healthcare020.WinUI.Helpers
{
    public class ConnectionCheck
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204")) 
                    return true; 
            }
            catch
            {
                return false;
            }
        }
    }
}