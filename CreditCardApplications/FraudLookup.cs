using System;
using System.Collections.Generic;
using System.Text;

namespace CreditCardApplications
{
    public class FraudLookup
    {
        virtual public bool IsFraudRisk(CreditCardApplication application)
        {
            return application.LastName == "Smith";
        }
    }
}
