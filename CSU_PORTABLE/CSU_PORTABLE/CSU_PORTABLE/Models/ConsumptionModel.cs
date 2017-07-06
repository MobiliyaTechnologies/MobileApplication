using CSU_PORTABLE.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Models
{
    public class ConsumptionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Expected { get; set; }
        public string Consumed { get; set; }
        public string Overused { get; set; }
    }

    public class Premise
    {
        public int PremiseID { get; set; }
        public string PremiseName { get; set; }
        public string PremiseDesc { get; set; }
        public int OrganizationID { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double MonthlyConsumption { get; set; }
        public double MonthlyPrediction { get; set; }
    }

    public class Building
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string BuildingDesc { get; set; }
        public int PremiseID { get; set; }
        public double MonthlyConsumption { get; set; }
        public double MonthlyPrediction { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class Meter
    {
        public int Id { get; set; }
        public string PowerScout { get; set; }
        public string Name { get; set; }
        public double MonthlyConsumption { get; set; }
        public double MonthlyPrediction { get; set; }
    }

    public class B2CConfigManager
    {
        private static B2CConfigManager _instance;
        private B2CConfigManager()
        {

        }
        public static B2CConfigManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new B2CConfigManager();
            }
            return _instance;
        }

        B2CConfiguration configuration;
        public void Initialize(B2CConfiguration config)
        {
            B2CConfig b2cConfig = new B2CConfig();
            b2cConfig.ReInitialize(config);
        }

        public string GetB2CAADInstanceUrl()
        {
            string tokenUrl = string.Format(configuration.B2cAuthorizeURL, configuration.B2cTenant, configuration.B2cSignUpPolicy, configuration.B2cClientId, configuration.B2cRedirectUrl);
            return tokenUrl;
        }
        public string GetB2CTokenUrl(string accessCode)
        {
            string tokenUrl = string.Format(configuration.B2cTokenURL, configuration.B2cTenant, configuration.B2cSignInPolicy, configuration.B2cClientSecret, configuration.B2cClientId, accessCode);
            return tokenUrl;
        }

        public string GetChangePasswordURL()
        {
            return string.Format(configuration.B2cChangePasswordURL, configuration.B2cTenant, configuration.B2cChangePasswordPolicy, configuration.B2cClientId, configuration.B2cRedirectUrl);
        }


    }

    public class B2CConfiguration
    {
        public string B2cTenant { get; set; }
        public string B2cClientId { get; set; }
        public string B2cClientSecret { get; set; }
        public string B2cSignUpPolicy { get; set; }
        public string B2cSignInPolicy { get; set; }
        public string B2cChangePasswordPolicy { get; set; }
        public string B2cAuthorizeURL { get; set; }
        public string B2cTokenURL { get; set; }
        public string B2cTokenURLIOS { get; set; }
        public string B2cChangePasswordURL { get; set; }
        public string B2cRedirectUrl { get; set; }
    }
}
