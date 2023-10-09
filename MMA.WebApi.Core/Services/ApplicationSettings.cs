using MMA.WebApi.Shared.Interfaces.ApplicationSettings;
using System;

namespace MMA.WebApi.Core.Services
{
    public class ApplicationSettings : IApplicationSettings
    {


        private string _datetime;
        public string datetime
        {
            get
            {
                // test method
                if (string.IsNullOrEmpty(_datetime))
                    _datetime = DateTime.Now.ToLongTimeString();

                return _datetime;
            }

        }

    }
}
