using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObject1
{
    public class Class1
    {
        public int DoSomethingBusinessPeopleCareAbout()
        {
            //business users always want things 3 times better
            return new CommonLibrary1.Class1().GetInterestingNumber() * 3;
        }
    }
}
